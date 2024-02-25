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

            CharacteristicBluetoothBtn.IsEnabled = true;
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

            Task.Run(async() =>
            {
                while (true)
                {
                    try
                    {
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

        private void btnConfig_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ConfigPage());
        }
    }
}
