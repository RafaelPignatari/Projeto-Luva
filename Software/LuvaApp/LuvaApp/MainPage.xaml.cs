using LuvaApp.Helpers.BluetoothHelper;
using LuvaApp.Helpers;
using LuvaApp.Models;
using LuvaApp.ViewModels;
using LuvaApp.Views;
using LuvaApp.Helpers.AlertHelper;

namespace LuvaApp
{
    public partial class MainPage : ContentPage
    {
        PosicaoViewModel _posicaoViewModel;
        ConfigurationModel? _configurationModel;

        public MainPage()
        {
            InitializeComponent();

            _posicaoViewModel = new PosicaoViewModel();
            BindingContext = _posicaoViewModel;
            Task.Run(PreencheConfiguracaoModel);

            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void TraduzSinalBtn_Clicked(object sender, EventArgs e)
        {
            if (true)
                PreverRemote();
            else if (_configurationModel.Processamento == Models.Enums.EProcessamento.Local)
                PreverLocal();
        }

        private void PreverRemote()
        {
            Task.Run(async () =>
            {
                bool bleRetornou = false;

                try
                {
                    string values = await RecepcaoController.Instancia.GetValues(await BluetoothController.GetInstance());
                    SetLetter(await APIController.PreverValor(values));
                }
                catch (Exception ex)
                {
                    await MainThread.InvokeOnMainThreadAsync(async() => await DisplayAlert("Erro", ex.Message, "OK"));
                }
            });
        }

        private void PreverLocal()
        {
            Task.Run(async () =>
            {
                bool bleRetornou = false;

                try
                {
                    //var alerta = AlertHelper.MontaContentAlerta();
                    //MainThread.BeginInvokeOnMainThread(async () => await AlertHelper.ShowDialog(this,
                    //                                                "Conectando ao dispositivo",
                    //                                                "Aguarde enquanto o aplicativo se conecta ao dispositivo",
                    //                                                10000, ref bleRetornou));
                    string values = await RecepcaoController.Instancia.GetValues(await BluetoothController.GetInstance());
                    bleRetornou = true;
                    SetLetter(await IAEmbarcadaController.Instancia.Predicao(values));
                }
                catch (Exception ex)
                {
                    bleRetornou = true;
                    MainThread.BeginInvokeOnMainThread(async() => await DisplayAlert("Erro", ex.Message, "OK"));
                }
            });
        }

        private void btnSom_Clicked(object sender, EventArgs e)
        {
            bool novaPosicao  = !_posicaoViewModel.SomImage.EndsWith("1.png");
            _posicaoViewModel.SomImage = $"volume{Convert.ToInt32(novaPosicao)}.png";
        }

        private void SetLetter(string letra)
        {
            string letraPng  = $"letra_{letra.ToLower()}.png";
            _posicaoViewModel.LetraImagem = letraPng;
        }

        private void btnConfig_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ConfigPage());
        }

        private void btnTraining_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new TrainingPage());
        }

        private async Task PreencheConfiguracaoModel()
        {
            _configurationModel = await ConfigurationController.GetConfigurationAsync();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //TODO: Adicionar logs
        }
    }
}
