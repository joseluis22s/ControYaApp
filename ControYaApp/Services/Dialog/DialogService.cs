using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using ControYaApp.Views.Controls;

namespace ControYaApp.Services.Dialog
{
    public class DialogService : IDialogService
    {
        private LoadingPopUp loadingPopUp;

        public Task ShowToast(string message, ToastDuration duration, double textSize)
        {
            return Toast.Make(message, duration, textSize).Show();
        }
        public Task<bool> DisplayAlert(string title, string message, string accept, string cancel)
        {
            return AppShell.Current.DisplayAlert(title, message, accept, cancel);
        }
        public Task<string> DisplayActionSheet(string title, string cancel, string destruction, params string[] buttons)
        {
            return AppShell.Current.DisplayActionSheet(title, cancel, destruction, buttons);
        }

        public async Task ShowLoadingPopUp()
        {
            loadingPopUp = new();
            await AppShell.Current.CurrentPage.ShowPopupAsync(loadingPopUp);
        }

        public async Task HideLoadingPopUp()
        {
            if (loadingPopUp != null)
            {
                await loadingPopUp.CloseAsync();
                loadingPopUp = null;
            }
        }
    }
}
