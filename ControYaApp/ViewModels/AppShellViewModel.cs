using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace ControYaApp.ViewModels
{
    public partial class AppShellViewModel : ViewModelBase
    {

        public ICommand GoToLoginCommand { get; }

        public ICommand FlyoutShellCommand { get; }


        public AppShellViewModel()
        {
            GoToLoginCommand = new AsyncRelayCommand(GoToLoginAsync);
            FlyoutShellCommand = new RelayCommand(FlyoutShell);
        }
        private async Task GoToLoginAsync()
        {
            await Shell.Current.GoToAsync("//login");

        }
        private void FlyoutShell()
        {
            Shell.Current.FlyoutIsPresented = !Shell.Current.FlyoutIsPresented;
        }
    }
}
