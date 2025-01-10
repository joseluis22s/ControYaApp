using CommunityToolkit.Maui.Views;
using ControYaApp.ViewModels.Controls;

namespace ControYaApp.Views.Controls;

public partial class LoadingPopUp : Popup
{
    public LoadingPopUp(LoadingPopUpViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}