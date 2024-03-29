using CommunityToolkit.Mvvm.ComponentModel;

namespace LuvaApp.ViewModels
{
    internal partial class PosicaoViewModel : ObservableObject
    {
        [ObservableProperty]
        private string letrasIdentificadas = "Previsões passadas: ";
        [ObservableProperty]
        private string textoIdentificado = "";
        [ObservableProperty]
        private List<string> previsoesPassadas = new List<string>();
        [ObservableProperty]
        private string somImage = "volume1.png";
        [ObservableProperty]
        private string letraImagem = ".png";
    }
}
