using CommunityToolkit.Mvvm.ComponentModel;

namespace LuvaApp.ViewModels
{
    internal partial class PosicaoViewModel : ObservableObject
    {
        [ObservableProperty]
        private string letrasIdentificadasLbl = "Letras identificadas: ";
        [ObservableProperty]
        private string somImage = "volume1.png";
        [ObservableProperty]
        private string letraImagem = "letra_a.png";
    }
}
