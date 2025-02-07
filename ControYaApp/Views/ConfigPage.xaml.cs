using ControYaApp.ViewModels;

namespace ControYaApp.Views;

public partial class ConfigPage : ContentPage
{
    public ConfigPage(ConfigViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}