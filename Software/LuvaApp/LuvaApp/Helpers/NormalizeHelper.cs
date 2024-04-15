using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuvaApp.Helpers
{
    public class NormalizeHelper
    {
        private static float[] _media;
        private static float[] _desvioPadrao;

        public static void VerificaEPreencheVariaveisIniciais()
        {
            if (_media == null || _desvioPadrao == null)
                Task.Run(PreencheVariaveisIniciais);
        }

        /// <summary>
        /// Normaliza os dados como é feito no StandarScaler do Scikit-Learn.
        /// </summary>
        /// <param name="dados"></param>
        /// <returns></returns>
        public static float[] NormalizaDados(float[] dados)
        {
            float[] dadosNormalizados = new float[dados.Length];

            for (int i = 0; i < dados.Length; i++)
            {
                dadosNormalizados[i] = (dados[i] - _media[i]) / _desvioPadrao[i];
            }

            return dadosNormalizados;
        }

        private static async Task PreencheVariaveisIniciais()
        {
            int qtdColunas = 0;

            using (var fileStream = await FileSystem.Current.OpenAppPackageFileAsync(Constantes.MODELO_TREINO))
            {
                using (var reader = new StreamReader(fileStream))
                {                    
                    var listaValores = new List<float[]>();

                    foreach (var linha in reader.ReadToEnd().Split('\n'))
                    {
                        qtdColunas = qtdColunas == 0 ? linha.Split(',').Length : qtdColunas;
                        var colunas = new float[qtdColunas];
                        var valores = linha.Split(',');
                        
                        //O último valor é o target, por isso -1
                        for (int i = 0; i < valores.Length - 1; i++)
                        {
                            colunas[i] += float.Parse(valores[i]);
                        }

                        listaValores.Add(colunas);
                    }

                    CalculaMedia(listaValores, qtdColunas);
                    CalculaDesvioPadrao(listaValores, qtdColunas);                    
                }
            }
        }

        private static void CalculaMedia(List<float[]> listaValores, int qtdColunas)
        {
            int qtdLinhas = 0;
            _media = new float[qtdColunas - 1];

            foreach (var linha in listaValores)
            {
                for (int i = 0; i < linha.Length - 1; i++)
                {
                    _media[i] += linha[i];
                }

                qtdLinhas++;
            }

            for (int i = 0; i < _media.Length - 1; i++)
            {
                _media[i] /= qtdLinhas;
            }
        }

        private static void CalculaDesvioPadrao(List<float[]> listaValores, int qtdColunas)
        {
            int qtdLinhas = 0;
            _desvioPadrao = new float[qtdColunas - 1];

            foreach (var linha in listaValores)
            {
                for (int i = 0; i < linha.Length - 1; i++)
                {
                    _desvioPadrao[i] += (float)Math.Pow(linha[i] - _media[i], 2);
                }

                qtdLinhas++;
            }

            for (int i = 0; i < _desvioPadrao.Length - 1; i++)
            {
                _desvioPadrao[i] = (float)Math.Sqrt(_desvioPadrao[i] / qtdLinhas);
            }
        }
    }
}
