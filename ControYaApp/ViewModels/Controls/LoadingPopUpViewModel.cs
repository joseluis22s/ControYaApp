using CommunityToolkit.Maui.Core;
using ControYaApp.Services.Navigation;
using ControYaApp.ViewModels.Base;

namespace ControYaApp.ViewModels.Controls
{
    public partial class LoadingPopUpViewModel : BaseViewModel
    {
        private readonly IPopupService _popupService;

        public LoadingPopUpViewModel(INavigationService navigationService, IPopupService popupService, Func<Task> operacion) : base(navigationService)
        {
            _popupService = popupService;
            EjecutarOperacion(operacion).GetAwaiter();
        }

        private async Task EjecutarOperacion(Func<Task> operacion)
        {
            try
            {
                if (operacion != null)
                {
                    await operacion();
                }
            }
            finally
            {
                await _popupService.ClosePopupAsync();
            }
        }
    }
}
