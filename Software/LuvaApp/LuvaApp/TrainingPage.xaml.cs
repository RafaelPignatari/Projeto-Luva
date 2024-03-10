using LuvaApp.Helpers;
using LuvaApp.Helpers.BluetoothHelper;
using System.Text;

namespace LuvaApp;

public partial class TrainingPage : ContentPage
{
    public TrainingPage()
	{
		InitializeComponent();
    }

    private async void OnComecarClicked(object sender, EventArgs e)
    {
        string letterToTrain = LetterToTrain.Text;

        string value = await RecepcaoController.Instancia.GetValues(await BluetoothController.GetInstance()) + $",{letterToTrain}";
        APIController.AdicionarDadosParaTreino(value);
        MainThread.BeginInvokeOnMainThread(() => DisplayAlert("Letter to Train", $"You entered: {letterToTrain}", "OK"));
    }
    private async void OnTreinarClicked(object sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(async () => DisplayAlert("Modelo treinado", $"Modelo treinado com precisão de {await APIController.TreinarModelo("svm")}", "OK"));
    }
}