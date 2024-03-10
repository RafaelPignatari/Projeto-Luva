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
            if (_configurationModel.Processamento == Models.Enums.EProcessamento.Local)
                PreverRemote();
            else if (_configurationModel.Processamento == Models.Enums.EProcessamento.Remoto)
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
                //string values = "2375,2519,2195,2394,1158,0017,-102,0000,0000,2379,2512,2217,2385,1166,0018,-106,0000,0000,2382,2526,2219,2384,1169,0017,-108,0000,0000,2371,2525,2224,2376,1161,0014,-109,0000,0000,2382,2522,2220,2361";

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
