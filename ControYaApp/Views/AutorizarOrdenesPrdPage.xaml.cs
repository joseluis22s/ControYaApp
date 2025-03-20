using ControYaApp.ViewModels;

namespace ControYaApp.Views;

public partial class AutorizarOrdenesPrdPage : ContentPage
{
    public AutorizarOrdenesPrdPage(AutorizarOrdenesPrdViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}