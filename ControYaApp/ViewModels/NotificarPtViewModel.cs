using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace ControYaApp.ViewModels
{
    public class NotificarPtViewModel : ViewModelBase
    {


        public ICommand GoBackCommand { get; }
        public NotificarPtViewModel()
        {
            GoBackCommand = new AsyncRelayCommand(GoBackAsync);
        }

        private async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
