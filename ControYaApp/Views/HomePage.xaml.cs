using ControYaApp.ViewModels;

namespace ControYaApp.Views;

public partial class HomePage : ContentPage
{
    public HomePage(HomeViewModel vm, AppShellViewModel appShellvm)
    {
        InitializeComponent();

        BindingContext = vm;

        ImageButton_FlyoutShell.Command = appShellvm.FlyoutShellCommand;
    }


    protected override bool OnBackButtonPressed()
    {
        // return true: Deshabilita comportamiento
        // return false: Mantiene comportamiento 
        // Use the line above if you want to just disable the Back action. 
        // If you want to instead bind it to the same command as 
        // the BackButtonBehavior, use something like this :

        if (BindingContext is HomeViewModel vm)
        {
            vm.BackButtonPressed().GetAwaiter();
            return true;
        }
        return false;
    }


}