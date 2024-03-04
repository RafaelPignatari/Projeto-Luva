using LuvaApp.Helpers.BluetoothHelper;
using LuvaApp.Helpers;
using LuvaApp.Models;
using LuvaApp.ViewModels;
using LuvaApp.Views;

namespace LuvaApp
{
    public partial class MainPage : ContentPage
    {
        BluetoothController bluetoothController = new BluetoothController();
        PosicaoViewModel _posicaoViewModel;

        public MainPage()
        {
            InitializeComponent();

            _posicaoViewModel = new PosicaoViewModel();
            BindingContext = _posicaoViewModel;
        }

        private async void ConnectToBluetoothDevice(object sender, EventArgs e)
        {
            await bluetoothController.AsyncRequestBluetoothPermissions();
            await bluetoothController.AsyncConnectToDeviceByName("LuvaController");

            TraduzSinalBtn.IsEnabled = true;
        }

        private async void GetCharacteristicValue(object sender, EventArgs e)
        {
            await Task.Run(async() => await RecepcaoController.Instancia.IniciaRecepcao(bluetoothController));        
        }

        private void TraduzSinalBtn_Clicked(object sender, EventArgs e)
        {
            TraduzSinalBtn.IsEnabled = false;
            TraduzSinalBtn.Text = "Atualizando automaticamente predict";
            Task.Run(async () =>
            {
                while (true)
                {
                    await MainThread.InvokeOnMainThreadAsync(async () => await bluetoothController.AsyncRequestBluetoothPermissions());
                    await MainThread.InvokeOnMainThreadAsync(async () => await bluetoothController.AsyncConnectToDeviceByName("LuvaController"));
                    await RecepcaoController.Instancia.IniciaRecepcao(bluetoothController);

                    try
                    {
                        TraduzSinalBtn.IsEnabled = true;

                        var recebido = RecepcaoController.Instancia.ObtemUltimoValorRecebido();

                        var input = new OnnxInput
                        {
                            Sensores = new float[] { recebido.Flexao2, recebido.Acc_EixoX, recebido.Acc_EixoY },
                        };

                        var predict = await IAEmbarcadaController.Instancia.Predicao(input);
                        await MainThread.InvokeOnMainThreadAsync(() => _posicaoViewModel.Posicao = predict);
                    }
                    catch (Exception ex)
                    {
                        //TODO: Adicionar logs
                    }

                    Thread.Sleep(50);
                }
            });
        }

        private void btnSom_Clicked(object sender, EventArgs e)
        {
            bool novaPosicao  = !_posicaoViewModel.SomImage.EndsWith("1.png");
            _posicaoViewModel.SomImage = "volume" + Convert.ToInt32(novaPosicao) + ".png";
        }

        private void randomLetter()
        {
            string letraPng  = "letra_" + (char)new Random().Next(97, 123) + ".png";
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
    }
}
