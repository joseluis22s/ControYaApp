using CommunityToolkit.Maui.Core;

namespace ControYaApp.ViewModels.Controls
{
    public class LoadingPopUpViewModel : ViewModelBase
    {
        private readonly IPopupService _popupService;

        public LoadingPopUpViewModel(IPopupService popupService, Func<Task> operacion)
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
