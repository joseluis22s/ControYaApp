using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace ControYaApp.ViewModels
{
    public partial class AppShellViewModel : ViewModelBase
    {

        public ICommand GoToLoginCommand { get; }
        public AppShellViewModel()
        {

            GoToLoginCommand = new AsyncRelayCommand(GoToLoginAsync);
        }
        private async Task GoToLoginAsync()
        {
            await Shell.Current.GoToAsync("//login");

        }
    }
}
