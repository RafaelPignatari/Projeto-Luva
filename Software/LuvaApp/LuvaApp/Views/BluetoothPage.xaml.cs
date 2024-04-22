using LuvaApp.Helpers.BluetoothHelper;
using LuvaApp.ViewModels;
using Plugin.BLE.Abstractions.Contracts;

namespace LuvaApp.Views;

public partial class BluetoothPage : ContentPage
{
    BluetoothViewModel _bluetoothViewModel;
    IEnumerable<IDevice> devicesFound;
    string dispositivoSelecionado;

    public BluetoothPage()
    {
        InitializeComponent();

        _bluetoothViewModel = new BluetoothViewModel();
        BindingContext = _bluetoothViewModel;

    }
    protected override void OnAppearing()
    {
        base.OnAppearing();

        var viewModel = (BluetoothViewModel)BindingContext;
        int rowIndex = dispositivosEncontradosGrid.RowDefinitions.Count;

        dispositivosEncontradosGrid.Children.Clear();

        foreach (var dispositivo in viewModel.DispositivosLocalizados)
        {
            dispositivosEncontradosGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var nomeDispositivoLabel = new Label
            {
                Text = dispositivo,
                FontSize = 18,
                TextColor = Color.FromArgb("#000000"),
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
            };

            nomeDispositivoLabel.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    foreach (var child in dispositivosEncontradosGrid.Children)
                    {
                        if (child is Label otherLabel)
                        {
                            otherLabel.BackgroundColor = Color.FromArgb("F5F5F5");
                        }
                    }

                    nomeDispositivoLabel.BackgroundColor = Color.FromArgb("#c7c5c5"); 
                    dispositivoSelecionado = nomeDispositivoLabel.Text;
                })
            });

            Grid.SetRow(nomeDispositivoLabel, rowIndex);
            dispositivosEncontradosGrid.Children.Add(nomeDispositivoLabel);

            var boxViewInferior = new BoxView
            {
                Color = Color.FromArgb("#000000"),
                HeightRequest = 1,
                VerticalOptions = LayoutOptions.End
            };

            Grid.SetRow(boxViewInferior, rowIndex);
            Grid.SetColumn(boxViewInferior, 0);
            dispositivosEncontradosGrid.Children.Add(boxViewInferior);

            rowIndex++; 
        }
    }

    private async void btnConectar_Clicked(object sender, EventArgs e)
    {
        if (String.IsNullOrEmpty(dispositivoSelecionado))
        {
            await DisplayAlert("Erro", "Por favor, selecione um dispositivo antes de conectar.", "OK");
            return;
        }
        else if (dispositivoSelecionado != "LuvaController")
        {
            await DisplayAlert("Erro", "Esta aplicação só pode ser conectada a dispositivos do tipo \"LuvaController\".", "OK");
            return;
        }
        else
        {
            BluetoothController bluetoothController = await BluetoothController.GetInstance2();
            await bluetoothController.AsyncConnectToDeviceByName2(dispositivoSelecionado, devicesFound);
            _bluetoothViewModel.DispositivoSelecionado = dispositivoSelecionado;
        }

    }

    private async void btnProcurarDispositivos_Clicked(object sender, EventArgs e)
    {
        BluetoothController bluetoothController = await BluetoothController.GetInstance2();
        devicesFound = await bluetoothController.AsyncGetDevices();
        _bluetoothViewModel.DispositivosLocalizados = devicesFound.Select(device => device.Name).ToList();
        OnAppearing();
    }
}
