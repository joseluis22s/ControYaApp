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
        // `return true`: Deshabilita comportamiento
        // `return false`: Mantiene comportamiento 
        if (BindingContext is HomeViewModel vm)
        {
            vm.BackButtonPressed().GetAwaiter();
            return true;
        }
        return false;
    }


}