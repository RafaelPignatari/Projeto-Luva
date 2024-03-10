using LuvaApp.Helpers;
using LuvaApp.Models;
using LuvaApp.ViewModels;

namespace LuvaApp.Views;

public partial class ConfigPage : ContentPage
{
    ConfigurationViewModel? configurationViewModel;
    public ConfigPage()
    {
        InitializeComponent();
        
         ConfigurationModel configurationModel = ConfigurationController.GetConfigurationAsync().Result;
        configurationViewModel = new ConfigurationViewModel(configurationModel);
        BindingContext = configurationViewModel;
    }

    private async void btnConfigBluetooth_Clicked(object sender, EventArgs e)
    {
        //TODO: Navigate to Bluetoth configuration
    }

    private async void btnSalvar_Clicked(object sender, EventArgs e)
    {
        if (!configurationViewModel.HistoricoAtivo)
            configurationViewModel.PrevisoesExibidasNoHistorico = 0;

        await ConfigurationController.SetConfigurationAsync(new ConfigurationModel(configurationViewModel));

        await DisplayAlert(Constantes.SUCESSO, Constantes.MSG_CONFIG_SALVA, Constantes.OK);
        await Navigation.PopAsync();
    }
}