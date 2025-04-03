using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using ControYaApp.Services.DI;
using ControYaApp.Services.LocalDatabase.Repositories;
using ControYaApp.Services.SharedData;
using ControYaApp.Services.WebService;
using ControYaApp.ViewModels.Base;
using ControYaApp.Views.Controls;

namespace ControYaApp.ViewModels
{
    public partial class AppShellViewModel : BaseViewModel
    {
        private readonly RestService _restService;

        private readonly LocalRepoService _localRepoService;

        private DataConfigRepo _dataConfigRepo;



        private bool _isConected;
        public bool IsConected
        {
            get => _isConected;
            set => SetProperty(ref _isConected, value);
        }



        public ISharedData SharedData { get; set; }



        public ICommand GoToLoginCommand { get; }

        public ICommand FlyoutShellCommand { get; }

        public ICommand GetAndSaveDataCommand { get; }




        public AppShellViewModel(DataConfigRepo dataConfigRepo, RestService restService, LocalRepoService localRepoService, ISharedData sharedData)
        {
            _restService = restService;
            _localRepoService = localRepoService;
            _dataConfigRepo = dataConfigRepo;

            SharedData = sharedData;

            InitIpAddress();

            GoToLoginCommand = new AsyncRelayCommand(GoToLoginAsync);
            FlyoutShellCommand = new RelayCommand(FlyoutShell);
            GetAndSaveDataCommand = new AsyncRelayCommand(GetAndSaveDataAsync);
        }




        private async void InitIpAddress()
        {
            var dataConfig = await _dataConfigRepo.GetDataConfigAsync();
            if (dataConfig is null)
            {
                SharedData.IpAddress = "";
                SharedData.Protocolo = "http://";
                SharedData.AutoApproveProduccion = false;
                SharedData.AutoApproveInventario = false;
            }
            else
            {
                SharedData.IpAddress = dataConfig.Ip;
                SharedData.Protocolo = dataConfig.Protocolo;
                SharedData.AutoApproveProduccion = dataConfig.AutoApproveProduccion;
                SharedData.AutoApproveInventario = dataConfig.AutoApproveInventario;

            }
        }

        private async Task GoToLoginAsync()
        {
            await Shell.Current.GoToAsync("//login");
        }


        private void FlyoutShell()
        {
            Shell.Current.FlyoutIsPresented = !Shell.Current.FlyoutIsPresented;
            IsConected = Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
        }

        private async Task GetAndSaveDataAsync()
        {
            var loadingPopUpp = new LoadingPopUp();
            _ = Shell.Current.CurrentPage.ShowPopupAsync(loadingPopUpp);

            try
            {
                //WeakReferenceMessenger.Default.Send(new ClearDataMessage("Vaciar"));

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
                await loadingPopUpp.CloseAsync();
            }
        }

        //private async Task




    }
}
