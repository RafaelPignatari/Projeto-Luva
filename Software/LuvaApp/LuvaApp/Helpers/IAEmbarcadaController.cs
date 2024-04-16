using LuvaApp.Models;
using Microsoft.ML.OnnxRuntime;

namespace LuvaApp.Helpers
{
    public class IAEmbarcadaController
    {
        private InferenceSession _sessaoMelhorAlgoritmo;
        private List<InferenceSession> _sessoesAlgoritmos;

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

        private string EfetuaPredict(OnnxInput entrada, bool melhorModelo)
        {
            //Se for o melhor modelo, já retorna de cara.
            if (melhorModelo)
                return ObtemValorFromModel(_sessaoMelhorAlgoritmo, entrada.Sensores);

            //Caso contrário, fazemos as tratativas para cada modelo.
            string mensagem = string.Empty;

            foreach (var sessao in _sessoesAlgoritmos)
            {
                mensagem += ObtemValorFromModel(sessao, entrada.Sensores);
                mensagem += ";";
            }

            //Remove o último ;
            mensagem = mensagem.Remove(mensagem.Length - 1);

            return MetodosShared.ValoresMaisRepetidas(mensagem);
        }

        public async Task<string> Predicao(string values, bool melhorModelo)
        {
            var valoresSeparados = values.Split(',');
            var valoresSeparadosFloat = valoresSeparados.Where(v => v != string.Empty).Select(float.Parse).ToArray();
            OnnxInput entrada = new OnnxInput { Sensores = valoresSeparadosFloat};

            try
            {
                await IniciaPredictionEngine(melhorModelo);
                return EfetuaPredict(entrada, melhorModelo);
            }
            catch (Exception ex)
            {
                //TODO: Adicionar logs
                return "0";
            }
        }

        private string ObtemValorFromModel(InferenceSession sessao, float[] input)
        {
            string retorno;
            using var inputOrtValue = OrtValue.CreateTensorValueFromMemory(NormalizeHelper.NormalizaDados(input),
                                                                           new long[] { 1, 69 });

            var inputs = new Dictionary<string, OrtValue>
            {
                { "float_input", inputOrtValue }
            };

            using var runOptions = new RunOptions();
            using (var outputs = sessao.Run(runOptions, inputs, sessao.OutputNames))
            {
                var output = outputs.First();
                retorno = output.GetStringTensorAsArray()[0];
            }

            return retorno;
        }

        private async Task IniciaPredictionEngine(bool melhorModelo)
        {
            if (_sessaoMelhorAlgoritmo == null)
            {
                if (melhorModelo)
                {
                    await PreparaParaInferenceMelhor();
                }
                else
                {
                    await PreparaParaInferenceAll();
                }
            }
        }

        private async Task PreparaParaInferenceMelhor()
        {
            if (_sessaoMelhorAlgoritmo == null)
            {
                var caminho = await ArquivoHelper.TrataArquivoRandomForest();

                _sessaoMelhorAlgoritmo = new InferenceSession(caminho);
            }
        }

        private async Task PreparaParaInferenceAll()
        {
            if (_sessoesAlgoritmos == null)
            {
                _sessoesAlgoritmos = new List<InferenceSession>();
                var caminhos = await ArquivoHelper.TrataArquivosModelo();

                foreach (var caminho in caminhos)
                {
                    _sessoesAlgoritmos.Add(new InferenceSession(caminho));
                }
            }
        }
    }
}
