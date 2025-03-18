using ControYaApp.ViewModels;

namespace ControYaApp.Views;

public partial class NotificarPmPage : ContentPage
{
    public NotificarPmPage(NotificarPmViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}