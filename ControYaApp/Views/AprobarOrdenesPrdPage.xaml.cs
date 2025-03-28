using ControYaApp.ViewModels;

namespace ControYaApp.Views;

public partial class AprobarOrdenesPrdPage : ContentPage
{
    public AprobarOrdenesPrdPage(AprobarOrdenesPrdViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}