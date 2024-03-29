using LuvaApp.Helpers;
using LuvaApp.Models;
using LuvaApp.ViewModels;

namespace LuvaApp.Views;

public partial class ConfigPage : ContentPage
{
    public ConfigPage()
    {
        InitializeComponent();

        Task.Run(ObtemConfigurationViewModel);
        
        BindingContext = new ConfigurationViewModel();
    }

    private async Task ObtemConfigurationViewModel() 
    {
        BindingContext = new ConfigurationViewModel(await ConfigurationController.GetConfigurationAsync());        
    }

    private async void btnConfigBluetooth_Clicked(object sender, EventArgs e)
    {
        //TODO: Navigate to Bluetoth configuration
    }

    private async void btnSalvar_Clicked(object sender, EventArgs e)
    {
        var configurationViewModel = BindingContext as ConfigurationViewModel;

        if (!configurationViewModel.HistoricoAtivo)
            configurationViewModel.PrevisoesExibidasNoHistorico = 50;

        await ConfigurationController.SetConfigurationAsync(new ConfigurationModel(configurationViewModel));

        await DisplayAlert(Constantes.SUCESSO, Constantes.MSG_CONFIG_SALVA, Constantes.OK);
        await Navigation.PopAsync();
    }
}