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
        try
        {
            string letterToTrain = LetterToTrain.Text;

            string value = await RecepcaoController.Instancia.GetValues(await BluetoothController.GetInstance()) + $",{letterToTrain}";
            APIController.AdicionarDadosParaTreino(value);

            MainThread.BeginInvokeOnMainThread(() => DisplayAlert("Letter to Train", $"You entered: {letterToTrain}", "OK"));
        }
        catch (Exception ex)
        {
            MainThread.BeginInvokeOnMainThread(() => DisplayAlert("Erro", ex.Message, "OK"));
        }
    }
    private void OnTreinarClicked(object sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            try
            {
                var precisao = await APIController.TreinarModelo();
                await DisplayAlert("Modelo treinado", $"Modelo treinado com precisão de {precisao}", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", ex.Message, "OK");
            }
        });
    }
}