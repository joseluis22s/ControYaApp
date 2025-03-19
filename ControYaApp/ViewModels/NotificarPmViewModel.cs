using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace ControYaApp.ViewModels
{
    public class NotificarPmViewModel : BaseViewModel
    {
        public ICommand NotificarPmCommand { get; }


        public NotificarPmViewModel()
        {
            NotificarPmCommand = new AsyncRelayCommand(NotificarPm);
        }


        private async Task NotificarPm()
        {

        }
    }
}
