using CommunityToolkit.Maui.Core;

namespace ControYaApp.ViewModels.Controls
{
    public class LoadingPopUpViewModel : ViewModelBase
    {
        readonly IPopupService popupService;

        public LoadingPopUpViewModel(IPopupService popupService)
        {
            this.popupService = popupService;
        }
    }
}
