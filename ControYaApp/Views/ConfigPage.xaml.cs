using ControYaApp.ViewModels;

namespace ControYaApp.Views;

public partial class ConfigPage : ContentPage
{
    public ConfigPage(ConfigViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override bool OnBackButtonPressed()
    {
        // Use the line above if you want to just disable the Back action. 
        // If you want to instead bind it to the same command as 
        // the BackButtonBehavior, use something like this :

        if (BindingContext is ConfigViewModel vm)
        {
            vm.BackButtonPressed().GetAwaiter();
            return true;
        }
        return false;
    }

    private async void BackButtonPressed()
    {
        if (BindingContext is ConfigViewModel vm)
        {
            await vm.BackButtonPressed();
        }
    }
}