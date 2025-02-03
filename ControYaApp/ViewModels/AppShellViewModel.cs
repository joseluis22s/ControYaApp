using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using ControYaApp.Services.DI;
using ControYaApp.Services.WebService;

namespace ControYaApp.ViewModels
{
    public partial class AppShellViewModel : ViewModelBase
    {
        private readonly RestService _restService;

        private readonly LocalRepoService _localRepoService;



        private bool _isConected;


        public bool IsConected
        {
            get => _isConected;
            set => SetProperty(ref _isConected, value);
        }



        public ICommand GoToLoginCommand { get; }

        public ICommand FlyoutShellCommand { get; }

        public ICommand ExtraerDatosCommand { get; }

        public AppShellViewModel(RestService restService, LocalRepoService localRepoService
            )
        {
            GoToLoginCommand = new AsyncRelayCommand(GoToLoginAsync);
            FlyoutShellCommand = new RelayCommand(FlyoutShell);
            ExtraerDatosCommand = new AsyncRelayCommand(ExtraerDatosAsync);

            _restService = restService;
            _localRepoService = localRepoService;
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

        public async Task ExtraerDatosAsync()
        {
            await Shell.Current.DisplayAlert("¿Seguro que desea extraer datos?", "Se sobreescribiran los que están actualmente guardados", "Aceptar", "Cancelar");

            var usuarios = await _restService.GetAllUsuariosAsync();
            var ordenes = await _restService.GetOrdenesProduccionAsync();
            var periodos = await _restService.GetAllPeriodosAsync();
            var productos = await _restService.GetAllPt();
            var materiales = await _restService.GetAllEm();
            var empleados = await _restService.GetAllEmpleados();

            await _localRepoService.EmpleadosRepo.SaveAllEmpleadosAsync(empleados);
            await _localRepoService.MaterialEgresadoRepo.SaveAllEmAsync(materiales);
            await _localRepoService.ProductoTerminadoRepo.SaveAllPtAsync(productos);
            await _localRepoService.PeriodoRepo.SaveAllPeriodosAsync(periodos);
            await _localRepoService.OrdenRepo.SaveAllOrdenesAsync(ordenes);
            await _localRepoService.UsuarioRepo.SaveAllUsuariosAsync(usuarios);


            // TODO: Extraer todos los datos aquí.
        }
    }
}
