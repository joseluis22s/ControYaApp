using ControYaApp.ViewModels;

namespace ControYaApp.Views;

public partial class OrdenesPage : ContentPage
{
    private readonly OrdenesViewModel _vm;
    public OrdenesPage(OrdenesViewModel vm, AppShellViewModel appShellvm)
    {
        InitializeComponent();

        BindingContext = _vm = vm;

        ImageButton_FlyoutShell.Command = appShellvm.FlyoutShellCommand;
    }

    private void ContentPage_Appearing(object sender, EventArgs e)
    {
        _vm.Appearing();
    }

    protected override bool OnBackButtonPressed()
    {
        // Use the line above if you want to just disable the Back action. 
        // If you want to instead bind it to the same command as 
        // the BackButtonBehavior, use something like this :

        if (BindingContext is OrdenesViewModel vm)
        {
            vm.BackButtonPressed().GetAwaiter();
            return true;
        }
        return false;
    }

}