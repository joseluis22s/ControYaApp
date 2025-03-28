using ControYaApp.ViewModels;

namespace ControYaApp.Views;

public partial class AprobarOrdenesPrdInvPage : ContentPage
{
    public AprobarOrdenesPrdInvPage(AprobarOrdenesPrdInvViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}