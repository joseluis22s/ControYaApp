using ControYaApp.ViewModels.Controls;

namespace ControYaApp.Views.Controls;

public partial class NotificarPtPdfPage : ContentPage
{
    private readonly NotificarPtPdfViewModel _vm;

    public NotificarPtPdfPage(NotificarPtPdfViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        _vm.Loaded();
    }
}