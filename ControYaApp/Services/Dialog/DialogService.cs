using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using ControYaApp.Views.Controls;

namespace ControYaApp.Services.Dialog
{
    public class DialogService : IDialogService
    {
        private LoadingPopUp loadingPopUp;

        public Task ShowToastAsync(string message, ToastDuration duration, double textSize)
        {
            return Toast.Make(message, duration, textSize).Show();
        }
        public Task<bool> DisplayAlertAsync(string title, string message, string accept, string cancel)
        {
            return AppShell.Current.DisplayAlert(title, message, accept, cancel);
        }
        public Task<string> DisplayActionSheetAsync(string title, string cancel, string destruction, params string[] buttons)
        {
            return AppShell.Current.DisplayActionSheet(title, cancel, destruction, buttons);
        }
        public Task<string> DisplayPromptAsync(string title, string message, string accept = "OK", string cancel = "Cancel", string placeholder = null, int maxLength = -1, Keyboard keyboard = default(Keyboard), string initialValue = "")
        {
            return AppShell.Current.DisplayPromptAsync(title, cancel, accept, cancel, placeholder, maxLength, keyboard, initialValue);
        }

        public async Task ShowLoadingPopUpAsync()
        {
            loadingPopUp = new();
            await AppShell.Current.CurrentPage.ShowPopupAsync(loadingPopUp);
        }

        public async Task HideLoadingPopUpAsync()
        {
            if (loadingPopUp != null)
            {
                await loadingPopUp.CloseAsync();
                loadingPopUp = null;
            }
        }
    }
}
