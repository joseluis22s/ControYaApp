using ControYaApp.Services.Navigation;
using ControYaApp.Services.NotifyPropertyChanged;

namespace ControYaApp.ViewModels.Base
{
    public abstract class BaseViewModel : NotifyPropertyChanged
    {
        public INavigationService NavigationService { get; }

        public BaseViewModel(INavigationService navigationService)
        {
            NavigationService = navigationService;
        }
    }
}
