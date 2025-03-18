using ControYaApp.ViewModels;

namespace ControYaApp.Views;

public partial class NotificarPtPage : ContentPage
{
    public NotificarPtPage(NotificarPtViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

}