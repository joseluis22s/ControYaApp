using System.Windows.Input;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Input;
using ControYaApp.Services.AppLocalDatabase;
using ControYaApp.Services.Dialog;
using ControYaApp.Services.LocalDatabase;
using ControYaApp.Services.Navigation;
using ControYaApp.Services.SharedData;
using ControYaApp.Services.WebService;
using ControYaApp.ViewModels.Base;

namespace ControYaApp.ViewModels
{
    public partial class AppShellViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly AppDbReposService _appDbReposService;
        private readonly PrdDbReposService _prdDbReposService;

        private readonly RestService _restService;




        private bool _isConected;
        public bool IsConected
        {
            get => _isConected;
            set => SetProperty(ref _isConected, value);
        }



        public ISharedData SharedData { get; set; }



        public ICommand LogOutCommand { get; }

        public ICommand FlyoutShellCommand { get; }

        public ICommand GetAndSaveDataCommand { get; }




        public AppShellViewModel(INavigationService navigationServie, IDialogService dialogService,
            AppDbReposService appDbReposService, PrdDbReposService prdDbReposService,
            RestService restService,
            ISharedData sharedData) : base(navigationServie)
        {
            _dialogService = dialogService;
            _appDbReposService = appDbReposService;
            _prdDbReposService = prdDbReposService;


            _restService = restService;

            SharedData = sharedData;

            InitIpAddress();

            LogOutCommand = new AsyncRelayCommand(LogOutAsync);
            FlyoutShellCommand = new RelayCommand(FlyoutShell);
            GetAndSaveDataCommand = new AsyncRelayCommand(GetAndSaveDataAsync);
        }




        private async void InitIpAddress()
        {
            var dataConfig = await _appDbReposService.DataConfigRepo.GetDataConfigAsync();
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

        private async Task LogOutAsync()
        {
            await NavigationService.LogOutAsync();
        }


        private void FlyoutShell()
        {
            Shell.Current.FlyoutIsPresented = !Shell.Current.FlyoutIsPresented;
            IsConected = Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
        }

        private async Task GetAndSaveDataAsync()
        {
            _ = _dialogService.ShowLoadingPopUpAsync();

            try
            {
                //WeakReferenceMessenger.Default.Send(new ClearDataMessage("Vaciar"));
                await _dialogService.DisplayAlert("¿Seguro que desea extraer datos?", "Se sobreescribiran los que están actualmente guardados", "Aceptar", "Cancelar");

                var usuarios = await _restService.GetAllUsuariosAsync();
                var ordenesProduccion = await _restService.GetAllOrdenesProduccionAsync(SharedData.UsuarioSistema);
                var rangoPeriodos = await _restService.GetRangosPeriodos();
                var ordenesProduccionPt = await _restService.GetAllOrdenesProduccionPtAsync(SharedData.UsuarioSistema);
                var ordenesProduccionPm = await _restService.GetAllOrdenesProduccionPmAsync(SharedData.UsuarioSistema);
                var empleados = await _restService.GetAllEmpleadosAsync();

                await _appDbReposService.EmpleadosRepo.SaveAllEmpleadosAsync(empleados);
                await _appDbReposService.PeriodoRepo.SaveRangosPeriodosAsync(rangoPeriodos);
                await _appDbReposService.UsuarioRepo.SaveAllUsuariosAsync(usuarios);

                await _prdDbReposService.OrdenProduccionRepo.SaveAllOrdenesProduccionAsync(ordenesProduccion);
                await _prdDbReposService.OrdenProduccionMpRepo.SaveAllOrdenesProduccionPmAsync(ordenesProduccionPm);
                await _prdDbReposService.OrdenProduccionPtRepo.SaveAllOrdenesProduccionPtAsync(ordenesProduccionPt);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowToast(ex.Message, ToastDuration.Long);
            }
            finally
            {
                await _dialogService.HideLoadingPopUpAsync();
            }
        }


    }
}
