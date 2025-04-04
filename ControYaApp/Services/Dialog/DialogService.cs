using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace ControYaApp.Services.Dialog
{
    public class DialogService : IDialogService
    {
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


        //public Task ShowToast(string message = "Message", ToastDuration duration = ToastDuration.Short, double textSize = 14)
        //{
        //    return Toast.Make(message, duration, textSize).Show();
        //}
        //public Task<bool> DisplayAlert(string title = "Title", string message = "Message", string accept = "Accept", string cancel = "Cancel", FlowDirection flowDirection = FlowDirection.LeftToRight)
        //{
        //    return AppShell.Current.DisplayAlert(title, message, accept, cancel, flowDirection);
        //}
        //public Task<string> DisplayActionSheet(string title = "Title", string cancel = "Cancel", string destruction = null, FlowDirection flowDirection = FlowDirection.LeftToRight, params string[] buttons)
        //{
        //    return AppShell.Current.DisplayActionSheet(title, cancel, destruction, flowDirection, buttons);
        //}
    }
}
