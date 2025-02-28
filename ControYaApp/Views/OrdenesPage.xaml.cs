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
}