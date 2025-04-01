using ControYaApp.ViewModels;

namespace ControYaApp.Views;

public partial class AprobarOrdenesPrdInvPage : ContentPage
{
    public AprobarOrdenesPrdInvPage(AprobarOrdenesPrdInvViewModel vm, AppShellViewModel appShellvm)
    {
        InitializeComponent();
        BindingContext = vm;
        ImageButton_FlyoutShell.Command = appShellvm.FlyoutShellCommand;
    }
}