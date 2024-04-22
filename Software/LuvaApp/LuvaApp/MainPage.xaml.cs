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

            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            NormalizeHelper.VerificaEPreencheVariaveisIniciais();
            Task.Run(PreencheConfiguracaoModel);
        }

        private void TraduzSinalBtn_Clicked(object sender, EventArgs e)
        {
            TraduzSinalBtn.IsEnabled = false;
            TraduzSinalBtn.Text = "Traduzindo...";

            if (_configurationModel.Processamento == Models.Enums.EProcessamento.Remoto)
                PreverRemote();
            else if (_configurationModel.Processamento == Models.Enums.EProcessamento.Local)
                PreverLocal();
        }

        private void PreverRemote()
        {
            Task.Run(async () =>
            {
                try
                {
                    string values = await RecepcaoController.Instancia.GetValues(await BluetoothController.GetInstance());
                    await SetPrediction(await APIController.PreverValor(values, _configurationModel.MelhorModelo));
                }
                catch (Exception ex)
                {
                    await MainThread.InvokeOnMainThreadAsync(async() => await DisplayAlert("Erro", ex.Message, "OK"));
                }

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    TraduzSinalBtn.IsEnabled = true;
                    TraduzSinalBtn.Text = "Traduzir sinal";
                });
            });
        }

        private void PreverLocal()
        {
            Task.Run(async () =>
            {
                try
                {
                    string values = await RecepcaoController.Instancia.GetValues(await BluetoothController.GetInstance());
                    await SetPrediction(await IAEmbarcadaController.Instancia.Predicao(values, _configurationModel.MelhorModelo));
                }
                catch (Exception ex)
                {
                    MainThread.BeginInvokeOnMainThread(async() => await DisplayAlert("Erro", ex.Message, "OK"));
                }

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    TraduzSinalBtn.IsEnabled = true;
                    TraduzSinalBtn.Text = "Traduzir sinal";
                });
            });
        }

        private void btnSom_Clicked(object sender, EventArgs e)
        {
            bool novaPosicao  = !_posicaoViewModel.SomImage.EndsWith("1.png");
            _posicaoViewModel.SomImage = $"volume{Convert.ToInt32(novaPosicao)}.png";
        }

        private async Task SetPrediction(string prediction)
        {
            if (prediction.Length == 1)
                SetLetter(prediction);
            else
                SetText(prediction);

            SetPrevisoesPassadas(prediction);
            await EmiteSomLetra(prediction);
        }

        private void SetText(string text)
        {
            _posicaoViewModel.TextoIdentificado = text;
            _posicaoViewModel.LetraImagem = "";
            MainThread.BeginInvokeOnMainThread(() => { 
                borderEllipse.IsVisible = false;
                TextoIdentificadoLbl.IsVisible = true;
            });
        }

        private void SetLetter(string letra)
        {
            string letraPng = $"letra_{letra.ToLower()}.png";
            _posicaoViewModel.LetraImagem = letraPng;
            _posicaoViewModel.TextoIdentificado = "";
            MainThread.BeginInvokeOnMainThread(() => {
                borderEllipse.IsVisible = true;
                TextoIdentificadoLbl.IsVisible = false;
            });
        }

        private void SetPrevisoesPassadas(string prediction)
        {
            if (_posicaoViewModel.PrevisoesPassadas.Count == _configurationModel.PrevisoesExibidasNoHistorico)
                _posicaoViewModel.PrevisoesPassadas.RemoveAt(0);
            
            _posicaoViewModel.PrevisoesPassadas.Add(prediction);
            _posicaoViewModel.LetrasIdentificadas = "Previsões passadas: " + string.Join(',', _posicaoViewModel.PrevisoesPassadas.AsEnumerable().Reverse());
        }

        private async Task EmiteSomLetra(string letra)
        {
            if (_posicaoViewModel.SomImage.EndsWith("0.png"))
                return;
            await TextToSpeech.Default.SpeakAsync(letra);
        }

        private async void btnConfig_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ConfigPage());
        }

        private async void btnTraining_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TrainingPage());
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
