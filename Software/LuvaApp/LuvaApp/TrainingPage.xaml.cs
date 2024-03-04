using LuvaApp.Helpers;
using LuvaApp.Helpers.BluetoothHelper;
using System.Text;

namespace LuvaApp;

public partial class TrainingPage : ContentPage
{
    BluetoothController bluetoothController = new BluetoothController();

    public TrainingPage()
	{
		InitializeComponent();
    }

    private async void ConnectToBluetoothDevice(object sender, EventArgs e)
    {
        await bluetoothController.AsyncRequestBluetoothPermissions();
        await bluetoothController.AsyncConnectToDeviceByName("LuvaController");

        CharacteristicBluetoothBtn.IsEnabled = true;
    }
    private async void OnComecarClicked(object sender, EventArgs e)
    {
        string letterToTrain = LetterToTrain.Text;
        await Task.Run(async () => await RecepcaoController.Instancia.IniciaRecepcao(bluetoothController));
        List<string> valuesCapturedList = new List<string>();
        while (true)
        {
            valuesCapturedList = RecepcaoController.ListaRecepcaoCopy.ToList();
            if (valuesCapturedList.Count == 5)
                break;
        }

        string value = string.Join(",", valuesCapturedList).Replace(",,",",") + $",{letterToTrain}";
        sendDataThroughAPI(value);
        MainThread.BeginInvokeOnMainThread(() => DisplayAlert("Letter to Train", $"You entered: {letterToTrain}", "OK"));
    }

    public static async void sendDataThroughAPI(string dados)
    {
        string url = "http://127.0.0.1:5000/receiveValues";
        var myData = new
        {
            values = dados
        };
        string jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(myData);
        StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseBody);
                }
                else
                {
                    Console.WriteLine($"Erro: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocorreu um erro: {ex.Message}");
            }
        }
    }
}