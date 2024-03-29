using System.Text;

namespace LuvaApp.Helpers
{
    internal class APIController
    {
        const string URL = "http://127.0.0.1:5000/";

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

        public static async Task<string> PreverValor(string dados)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsync(URL + "predict", SetDadosToJson(dados));
                if (!response.IsSuccessStatusCode)
                    throw new Exception("API se escontra indisponível ou valores são inválidos");

                string returnMessage = await response.Content.ReadAsStringAsync();
                return GetMostRepeatedWord(returnMessage);
            }
        }

        private static string GetMostRepeatedWord(string message)
        {
            string[] words = message.Split(';');
            var wordGroups = words.GroupBy(w => w);
            var mostRepeatedWordGroup = wordGroups.OrderByDescending(g => g.Count()).First();
            return mostRepeatedWordGroup.Key;
        }

        private static StringContent SetDadosToJson(string dados = "", bool isBestModel = false)
        {
            var myData = new
            {
                isBestModel = isBestModel,
                values = dados.Split(',').Select(a => a.ToString()).ToArray()
            };
            string jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(myData);
            StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            return content;
        }
    }
}