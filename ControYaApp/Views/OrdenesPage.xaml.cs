using ControYaApp.ViewModels;

namespace ControYaApp.Views;

public partial class OrdenesPage : ContentPage
{
    public OrdenesPage(OrdenesViewModel vm, AppShellViewModel appShellvm)
    {
        InitializeComponent();

        BindingContext = vm;

        ImageButton_FlyoutShell.Command = appShellvm.FlyoutShellCommand;
    }
}