using CommunityToolkit.Maui.Core;

namespace ControYaApp.Services.Dialog
{
    public interface IDialogService
    {
        Task ShowToast(string message = "Message", ToastDuration duration = ToastDuration.Short, double textSize = 14);

        Task<bool> DisplayAlert(string title = "Title", string message = "Message", string accept = "Accept", string cancel = "Cancel");

        Task<string> DisplayActionSheet(string title = "Title", string cancel = "Cancel", string destruction = null, params string[] buttons);
    }
}
