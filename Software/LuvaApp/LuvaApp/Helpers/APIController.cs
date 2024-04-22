using System.Text;

namespace LuvaApp.Helpers
{
    internal class APIController
    {
        const string URL = "https://luvareconhecimento.azurewebsites.net/";
        //Para rodar a API localmente.
        //const string URL = "http://localhost:5000/";

        public static async void AdicionarDadosParaTreino(string dados)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsync(URL + "receiveValues", SetDadosToJson(dados));
                if (!response.IsSuccessStatusCode)
                    throw new Exception("API se escontra indisponível ou valores são inválidos");
            }
        }

        public static async Task<string> TreinarModelo()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsync(URL + "trainModel", SetDadosToJson());
                if (!response.IsSuccessStatusCode)
                    throw new Exception("API se escontra indisponível ou valores são inválidos");

                return await response.Content.ReadAsStringAsync();
            }
        }

        public static async Task<string> PreverValor(string dados, bool melhorModelo)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsync(URL + "predict", SetDadosToJson(dados, melhorModelo));
                if (!response.IsSuccessStatusCode)
                    throw new Exception("API se escontra indisponível ou valores são inválidos");

                string returnMessage = await response.Content.ReadAsStringAsync();
                return MetodosShared.ValoresMaisRepetidas(returnMessage);
            }
        }

        private static StringContent SetDadosToJson(string dados = "", bool melhorModelo = false)
        {
            var myData = new
            {
                isBestModel = melhorModelo,
                values = dados.Split(',').Select(a => a.ToString()).ToArray()
            };
            string jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(myData);
            StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            return content;
        }
    }
}