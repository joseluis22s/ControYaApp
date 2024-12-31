using ControYaApp.ViewModels;

namespace ControYaApp.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent(LoginViewModel loginViewModel);
        BindingContext = loginViewModel;

    }
}