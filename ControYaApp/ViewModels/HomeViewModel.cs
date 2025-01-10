using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace ControYaApp.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        public ICommand GoToLoginCommand { get; }

        public HomeViewModel()
        {
            GoToLoginCommand = new AsyncRelayCommand(GoToLogin);
        }

        private async Task GoToLogin()
        {

            await Shell.Current.GoToAsync("//login");
        }
    }
}
