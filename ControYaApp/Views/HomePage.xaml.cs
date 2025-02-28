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
}