using LuvaApp.Helpers.BluetoothHelper;
using LuvaApp.Helpers;
using LuvaApp.Models;
using LuvaApp.ViewModels;
using LuvaApp.Views;

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
                string values = await RecepcaoController.Instancia.GetValues(await BluetoothController.GetInstance());
                SetLetter(await APIController.PreverValor(values));
            });
        }

        private void PreverLocal()
        {
            Task.Run(async () =>
            {
                string values = await RecepcaoController.Instancia.GetValues(await BluetoothController.GetInstance());
                SetLetter(await IAEmbarcadaController.Instancia.Predicao(values));
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
            Exception exception = e.ExceptionObject as Exception;
            DisplayAlert("Unhandled Exception", exception?.Message, "OK");
        }
    }
}
