using ControYaApp.ViewModels;

namespace ControYaApp.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel vm)
    {
        InitializeComponent();

        BindingContext = vm;
    }



}
