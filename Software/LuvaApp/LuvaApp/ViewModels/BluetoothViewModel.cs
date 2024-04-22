using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuvaApp.ViewModels
{
    internal partial class BluetoothViewModel : ObservableObject
    {
        [ObservableProperty]
        private List<string> dispositivosLocalizados = new List<string>();
        [ObservableProperty]
        private string dispositivoSelecionado = "";
    }
}
