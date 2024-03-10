using LuvaApp.Models;
using Microsoft.ML;

namespace LuvaApp.Helpers
{
    public class IAEmbarcadaController
    {
        private PredictionEngine<OnnxInput, OnnxOutput> _predictionEngine;

        private IAEmbarcadaController()
        {
            MLContext mlContext = new MLContext();
            Task.Run(async() => _predictionEngine = await GetPredictionPipeline(mlContext));
        }

        #region SINGLETON
        private static IAEmbarcadaController _instancia;

        // Private constructor to prevent instance creation

        public static IAEmbarcadaController Instancia
        {
            get
            {
                if (_instancia == null)
                {
                    _instancia = new IAEmbarcadaController();
                }

                return _instancia;
            }
        }
        #endregion

        private async Task<PredictionEngine<OnnxInput, OnnxOutput>> GetPredictionPipeline(MLContext mLContext)
        {
            var inputColumns = new string[] { "float_input" };
            var outputColumns = new string[] { "label", "probabilities" };
            var caminho = await ArquivoHelper.TrataArquivosModelo();

            var onnxPredictionPipeline = mLContext.Transforms
                                                  .ApplyOnnxModel(outputColumnNames: outputColumns,
                                                                  inputColumnNames: inputColumns,
                                                                  caminho);
            var emptyDv = mLContext.Data.LoadFromEnumerable(new OnnxInput[] { });

            var model = onnxPredictionPipeline.Fit(emptyDv);

            return mLContext.Model.CreatePredictionEngine<OnnxInput, OnnxOutput>(model);
        }

        public async Task<string> Predicao(string values)
        {
            OnnxInput entrada = new OnnxInput { Sensores = values.Split(',').Select(value => float.Parse(value)).ToArray() };

            try
            {
                if (_predictionEngine == null)
                    return "0";

                var result = _predictionEngine.Predict(entrada);
                return result.Resultado[0].ToString();
            }
            catch (Exception ex)
            {
                //TODO: Adicionar logs
                return "0";
            }
        }

    }
}
