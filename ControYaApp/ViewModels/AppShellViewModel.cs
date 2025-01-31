using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using ControYaApp.Services.WebService;

namespace ControYaApp.ViewModels
{
    public partial class AppShellViewModel : ViewModelBase
    {

        private readonly RestService _restService;


        private bool _isConected;


        public bool IsConected
        {
            get => _isConected;
            set => SetProperty(ref _isConected, value);
        }



        public ICommand GoToLoginCommand { get; }

        public ICommand FlyoutShellCommand { get; }

        public ICommand SincronizarCommand { get; }

        public AppShellViewModel(RestService restService)
        {
            GoToLoginCommand = new AsyncRelayCommand(GoToLoginAsync);
            FlyoutShellCommand = new RelayCommand(FlyoutShell);
            SincronizarCommand = new AsyncRelayCommand(SincronizarAsync);

            _restService = restService;
        }
        private async Task GoToLoginAsync()
        {
            await Shell.Current.GoToAsync("//login");

        }
        private void FlyoutShell()
        {
            Shell.Current.FlyoutIsPresented = !Shell.Current.FlyoutIsPresented;

            NetworkAccess accessType = Connectivity.Current.NetworkAccess;
            if (accessType == NetworkAccess.Internet)
            {
                IsConected = true;
            }
            IsConected = false;
        }

        public async Task SincronizarAsync()
        {
            await Shell.Current.DisplayAlert("¡Alerta!", "¿Seguro que desea extraer datos? Se sobreescribiran los actualmente guardaos", "Aceptar", "Cancelar");

            var usuarios = _restService
        }
    }
}
