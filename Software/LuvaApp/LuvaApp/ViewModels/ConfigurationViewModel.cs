using CommunityToolkit.Mvvm.ComponentModel;
using LuvaApp.Models;
using LuvaApp.Models.Enums;

namespace LuvaApp.ViewModels
{   
    public partial class ConfigurationViewModel : ObservableObject
    {
        [ObservableProperty]
        private int processamento = 0;
        [ObservableProperty]
        private int emissaoSom = 0;
        [ObservableProperty]
        private bool historicoAtivo = false;
        [ObservableProperty]
        private int previsoesExibidasNoHistorico = 0;

        public ConfigurationViewModel() {}

        public ConfigurationViewModel(ConfigurationModel configurationModel)
        {
            processamento = (int)configurationModel.Processamento;
            emissaoSom = (int)configurationModel.EmissaoSom;
            historicoAtivo = configurationModel.HistoricoAtivo;
            previsoesExibidasNoHistorico = configurationModel.PrevisoesExibidasNoHistorico;
        }
    }
}
