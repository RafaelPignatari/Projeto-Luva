using CommunityToolkit.Mvvm.ComponentModel;

namespace LuvaApp.ViewModels
{
    internal partial class PosicaoViewModel : ObservableObject
    {
        [ObservableProperty]
        private string letrasIdentificadasLbl = "Letras identificadas: ";
        [ObservableProperty]
        private List<string> letrasPassadas = new List<string>();
        [ObservableProperty]
        private string somImage = "volume1.png";
        [ObservableProperty]
        private string letraImagem = ".png";
    }
}
