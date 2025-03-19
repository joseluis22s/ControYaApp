using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ControYaApp.Services.DI;
using ControYaApp.Services.LocalDatabase.Repositories;
using ControYaApp.Services.SharedData;
using ControYaApp.Services.WebService;
using ControYaApp.Views.Controls;

namespace ControYaApp.ViewModels
{
    public partial class AppShellViewModel : BaseViewModel
    {

        private readonly RestService _restService;

        private readonly LocalRepoService _localRepoService;

        private IpServidorRepo _ipServidorRepo;



        private bool _isConected;
        public bool IsConected
        {
            get => _isConected;
            set => SetProperty(ref _isConected, value);
        }



        public ISharedData SharedData { get; set; }



        public ICommand GoToLoginCommand { get; }

        public ICommand FlyoutShellCommand { get; }

        public ICommand ExtraerDatosCommand { get; }

        public AppShellViewModel(IpServidorRepo ipServidorRepo, RestService restService, LocalRepoService localRepoService, ISharedData sharedData)
        {

            SharedData = sharedData; //No mover.

            _restService = restService;
            _localRepoService = localRepoService;
            _ipServidorRepo = ipServidorRepo;

            InitIpAddress();

            GoToLoginCommand = new AsyncRelayCommand(GoToLoginAsync);
            FlyoutShellCommand = new RelayCommand(FlyoutShell);
            ExtraerDatosCommand = new AsyncRelayCommand(ExtraerDatosAsync);
        }



        private async void InitIpAddress()
        {
            var ip = await _ipServidorRepo.GetIpServidorAsync();
            if (ip is null)
            {
                SharedData.IpAddress = "";
                SharedData.Protocolo = "http://";
            }
            else
            {
                SharedData.IpAddress = ip.Ip;
                SharedData.Protocolo = ip.Protocolo;
            }
        }



        private async Task GoToLoginAsync()
        {
            await Shell.Current.GoToAsync("//login");

        }


        private void FlyoutShell()
        {
            Shell.Current.FlyoutIsPresented = !Shell.Current.FlyoutIsPresented;

            NetworkAccess accessType = Connectivity.Current.NetworkAccess;

            IsConected = false;
            if (accessType == NetworkAccess.Internet)
            {
                IsConected = true;
            }
        }

        private async Task ExtraerDatosAsync()
        {

            var loadingPopUpp = new LoadingPopUp();
            _ = Shell.Current.CurrentPage.ShowPopupAsync(loadingPopUpp);

            try
            {
                WeakReferenceMessenger.Default.Send(new ClearDataMessage("Vaciar"));


                await Shell.Current.DisplayAlert("¿Seguro que desea extraer datos?", "Se sobreescribiran los que están actualmente guardados", "Aceptar", "Cancelar");

                var usuarios = await _restService.GetAllUsuariosAsync();
                var ordenesProduccion = await _restService.GetAllOrdenesProduccionAsync(SharedData.UsuarioSistema);
                var rangoPeriodos = await _restService.GetRangosPeriodos();
                var ordenesProduccionPt = await _restService.GetAllOrdenesProduccionPtAsync(SharedData.UsuarioSistema);
                var ordenesProduccionPm = await _restService.GetAllOrdenesProduccionPmAsync(SharedData.UsuarioSistema);
                var empleados = await _restService.GetAllEmpleadosAsync();


                await _localRepoService.EmpleadosRepo.SaveAllEmpleadosAsync(empleados);
                await _localRepoService.OrdenProduccionMpRepo.SaveAllOrdenesProduccionPmAsync(ordenesProduccionPm);
                await _localRepoService.OrdenProduccionPtRepo.SaveAllOrdenesProduccionPtAsync(ordenesProduccionPt);
                await _localRepoService.PeriodoRepo.SaveRangosPeriodosAsync(rangoPeriodos);
                await _localRepoService.OrdenesProduccionRepo.SaveAllOrdenesProduccionAsync(ordenesProduccion);
                await _localRepoService.UsuarioRepo.SaveAllUsuariosAsync(usuarios);


            }
            catch (Exception ex)
            {
                await Toast.Make(ex.Message, ToastDuration.Long).Show();
            }
            finally
            {
                Shell.Current.FlyoutIsPresented = false;

                await loadingPopUpp.CloseAsync();
            }


        }




    }
}
