using ControYaApp.ViewModels;

namespace ControYaApp.Views;

public partial class HomePage : ContentPage
{
    public HomePage(HomeViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;

    }
}