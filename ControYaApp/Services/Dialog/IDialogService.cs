using CommunityToolkit.Maui.Core;

namespace ControYaApp.Services.Dialog
{
    public interface IDialogService
    {
        Task ShowToastAsync(string message = "Message", ToastDuration duration = ToastDuration.Short, double textSize = 14);

        Task<bool> DisplayAlertAsync(string title = "Title", string message = "Message", string accept = "Accept", string cancel = "Cancel");

        Task<string> DisplayActionSheetAsync(string title = "Title", string cancel = "Cancel", string destruction = null, params string[] buttons);

        Task<string> DisplayPromptAsync(string title, string message, string accept = "OK", string cancel = "Cancel", string placeholder = null, int maxLength = -1, Keyboard keyboard = default(Keyboard), string initialValue = "");

        Task ShowLoadingPopUpAsync();

        Task HideLoadingPopUpAsync();
    }
}
