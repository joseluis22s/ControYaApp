using ControYaApp.ViewModels;

namespace ControYaApp.Views;

public partial class AprobarOrdenesPrdPage : ContentPage
{
    public AprobarOrdenesPrdPage(AprobarOrdenesPrdViewModel vm, AppShellViewModel appShellvm)
    {
        InitializeComponent();

        BindingContext = vm;

        ImageButton_FlyoutShell.Command = appShellvm.FlyoutShellCommand;
    }
}