using Microsoft.Maui.Controls.PlatformConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static async Task<string> TreinarModelo(string model)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsync(URL + "trainModel", SetDadosToJson(model));
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

                return await response.Content.ReadAsStringAsync();
            }
        }

        private static StringContent SetDadosToJson(string dados, string modelSelected = "svm")
        {
            var myData = new
            {
                model = modelSelected,
                values = dados.Split(',').Select(a => a.ToString()).ToArray()
            };
            string jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(myData);
            StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            return content;
        }
    }
}
