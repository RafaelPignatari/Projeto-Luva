using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuvaApp.Helpers
{
    public static class ArquivoHelper
    {
        /// <summary>
        /// Se os arquivos de modelos não existirem, eles são copiados do pacote para o diretório de dados do aplicativo.
        /// </summary>
        /// <returns></returns>
        public static async Task<List<string>> TrataArquivosModelo()
        {
            var arquivos = new string[] { Constantes.RF_ONNX_MODEL_PATH, Constantes.DT_ONNX_MODEL_PATH, 
                                          Constantes.SVM_ONNX_MODEL_PATH, Constantes.LR_ONNX_MODEL_PATH, };
            var caminhos = new List<string>();

            foreach (var arquivo in arquivos)
            {
                var caminho = Path.Combine(FileSystem.AppDataDirectory, arquivo);

                if (!File.Exists(Path.Combine(FileSystem.AppDataDirectory, arquivo)))
                {
                    using (var fileStream = await FileSystem.Current.OpenAppPackageFileAsync(arquivo))
                    {
                        using (var ms = new MemoryStream())
                        {
                            await fileStream.CopyToAsync(ms);
                            byte[] bytes = ms.ToArray();

                            File.WriteAllBytes(caminho, bytes);
                        }
                    }
                }

                caminhos.Add(caminho.ToString());
            }

            return caminhos;
        }

        /// <summary>
        /// Se o arquivo modelo não existir, ele é copiado do pacote para o diretório de dados do aplicativo.
        /// </summary>
        /// <returns></returns>
        public static async Task<string> TrataArquivoRandomForest()
        {
            var caminho = Path.Combine(FileSystem.AppDataDirectory, Constantes.RF_ONNX_MODEL_PATH);

            if (!File.Exists(Path.Combine(FileSystem.AppDataDirectory, Constantes.RF_ONNX_MODEL_PATH)))
            {
                using (var fileStream = await FileSystem.Current.OpenAppPackageFileAsync(Constantes.RF_ONNX_MODEL_PATH))
                {
                    using (var ms = new MemoryStream())
                    {
                        await fileStream.CopyToAsync(ms);
                        byte[] bytes = ms.ToArray();

                        File.WriteAllBytes(caminho, bytes);
                    }
                }
            }

            return caminho;
        }
    }
}