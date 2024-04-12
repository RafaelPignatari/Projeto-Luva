using LuvaApp.Models.Enums;
using LuvaApp.ViewModels;

namespace LuvaApp.Models
{   
    public class ConfigurationModel
    {
        public EProcessamento Processamento { get; set; } = EProcessamento.Local;
        public EEmissaoSom EmissaoSom { get; set; } = EEmissaoSom.Automatico;
        public bool HistoricoAtivo { get; set; } = false;
        public int PrevisoesExibidasNoHistorico { get; set; } = 5;
        public bool MelhorModelo { get; set; } = false;

        public ConfigurationModel() { }
        public ConfigurationModel(ConfigurationViewModel configurationViewModel)
        {
            Processamento = (EProcessamento)configurationViewModel.Processamento;
            EmissaoSom = (EEmissaoSom)configurationViewModel.EmissaoSom;
            HistoricoAtivo = configurationViewModel.HistoricoAtivo;
            PrevisoesExibidasNoHistorico = configurationViewModel.PrevisoesExibidasNoHistorico;
            MelhorModelo = configurationViewModel.MelhorModelo;
        }
    }
}
