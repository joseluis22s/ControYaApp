using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Input;
using ControYaApp.Models;
using ControYaApp.Services.AppLocalDatabase;
using ControYaApp.Services.Dialog;
using ControYaApp.Services.LocalDatabase;
using ControYaApp.Services.Navigation;
using ControYaApp.Services.OrdenProduccionFilter;
using ControYaApp.Services.SharedData;
using ControYaApp.Services.WebService;
using ControYaApp.ViewModels.Base;

namespace ControYaApp.ViewModels
{
    public partial class HomeViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly AppDbReposService _appDbReposService;
        private readonly PrdDbReposService _prdDbReposService;


        private readonly RestService _restService;



        private OrdenProduccionFilter OrdenProduccionFilter { get; set; }

        public ISharedData SharedData { get; set; }


        private int _ptNotificadoDesyncCount;
        public int PtNotificadoDesyncCount
        {
            get => _ptNotificadoDesyncCount;
            set => SetProperty(ref _ptNotificadoDesyncCount, value);
        }

        private int _unapprovPtNotificadoPrdCount;
        public int UnapprovPtNotificadoPrdCount
        {
            get => _unapprovPtNotificadoPrdCount;
            set => SetProperty(ref _unapprovPtNotificadoPrdCount, value);
        }
        private int _unapprovPtNotificadoInvCount;
        public int UnapprovPtNotificadoInvCount
        {
            get => _unapprovPtNotificadoInvCount;
            set => SetProperty(ref _unapprovPtNotificadoInvCount, value);
        }


        private int _mpNotificadoDesyncCount;
        public int MpNotificadoDesyncCount
        {
            get => _mpNotificadoDesyncCount;
            set => SetProperty(ref _mpNotificadoDesyncCount, value);
        }


        private int _unapprovMpNotificadoPrdCount;
        public int UnapprovMpNotificadoPrdCount
        {
            get => _unapprovMpNotificadoPrdCount;
            set => SetProperty(ref _unapprovMpNotificadoPrdCount, value);
        }
        private int _unapprovMpNotificadoInvCount;
        public int UnapprovMpNotificadoInvCount
        {
            get => _unapprovMpNotificadoInvCount;
            set => SetProperty(ref _unapprovMpNotificadoInvCount, value);
        }


        public bool OrdenesGroupLoaded => !OrdenesGroupIsNull;

        // TODO: A lo mejor se eliminna esto dependideno del comportamiento de bool.
        private bool _ordenesGroupIsNull;
        public bool OrdenesGroupIsNull
        {
            get => _ordenesGroupIsNull;
            set
            {
                if (SetProperty(ref _ordenesGroupIsNull, value))
                {
                    OnPropertyChanged(nameof(OrdenesGroupLoaded));
                }
            }
        }


        private int _pendingOrdenesProdCount;
        public int PendingOrdenesProdCount
        {
            get => _pendingOrdenesProdCount;
            set => SetProperty(ref _pendingOrdenesProdCount, value);
        }


        private int _ordenesProdCount;
        public int OrdenesProdCount
        {
            get => _ordenesProdCount;
            set => SetProperty(ref _ordenesProdCount, value);
        }


        private int _ordenesProdMpCount;
        public int OrdenesProdMpCount
        {
            get => _ordenesProdMpCount;
            set => SetProperty(ref _ordenesProdMpCount, value);
        }



        public ICommand SincronizarOrdenesProduccionCommand { get; }


        public HomeViewModel(INavigationService navigationService, IDialogService dialogService,
            PrdDbReposService prdDbReposService, AppDbReposService appDbReposService,
            ISharedData sharedData, RestService restService) : base(navigationService)
        {
            _dialogService = dialogService;
            _appDbReposService = appDbReposService;
            _prdDbReposService = prdDbReposService;


            SharedData = sharedData;

            _restService = restService;

            SincronizarOrdenesProduccionCommand = new AsyncRelayCommand(SincronizarOrdenesProduccionAsync);

            InitData();
        }

        private async Task SincronizarOrdenesProduccionAsync()
        {
            NetworkAccess accessType = Connectivity.Current.NetworkAccess;

            if (accessType != NetworkAccess.Internet)
            {
                await _dialogService.ShowToast("Sin conexión. No se puede realizar esta acción", ToastDuration.Long);
                return;
            }

            _ = _dialogService.ShowLoadingPopUpAsync();

            try
            {
                List<PtNotificado> ptNotificadosNoSinc = await _prdDbReposService.PtNotificadoRepo.GetPtNotificadosNoSyncAsync() ?? new();
                List<MpNotificado> mpNotificadosNoSinc = await _prdDbReposService.MpNotificadoRepo.GetMpNotificadosNoSyncAsync() ?? new();

                // Cuando haya PT notficados (que AllPtNotificados no sea nullo y cuando no es nulo que al menos haya uno)
                // notifica los que esten con `NotificarManyPtAsync()` y luego los elimina con `DeleteAllPtNotificado()`.
                //
                // Lo mismo ocurre con MP notificados.
                if (ptNotificadosNoSinc is not null && ptNotificadosNoSinc.Count != 0)
                {
                    foreach (var item in ptNotificadosNoSinc)
                    {
                        item.Sincronizado = true;
                    }
                }

                if (mpNotificadosNoSinc is not null && mpNotificadosNoSinc.Count != 0)
                {
                    foreach (var item in mpNotificadosNoSinc)
                    {
                        item.Sincronizado = true;
                    }
                }

                var req = new
                {
                    PtNotificados = ptNotificadosNoSinc,
                    MpNotificados = mpNotificadosNoSinc
                };

                if (!await _restService.ProcessPtMpNotificados(req))
                {
                    await _dialogService.ShowToast("Error al sincronizar los PT y MP notificados", ToastDuration.Long);
                    return;
                }
                await _prdDbReposService.PtNotificadoRepo.DeleteAllPtNotificado();
                await _prdDbReposService.MpNotificadoRepo.DeleteAllMpNotificado();


                var usuarios = await _restService.GetAllUsuariosAsync();
                var ordenesProduccion = await _restService.GetAllOrdenesProduccionAsync(SharedData.UsuarioSistema);
                var rangoPeriodos = await _restService.GetRangosPeriodos();
                var ordenesProduccionPt = await _restService.GetAllOrdenesProduccionPtAsync(SharedData.UsuarioSistema);
                var ordenesProduccionPm = await _restService.GetAllOrdenesProduccionPmAsync(SharedData.UsuarioSistema);
                var empleados = await _restService.GetAllEmpleadosAsync();
                var unapprovedPtNoticados = await _restService.GetUnapproveddPtPrdInv(SharedData.UsuarioSistema);
                var unapprovedMpNoticados = await _restService.GetUnapproveddMpPrdInv(SharedData.UsuarioSistema);

                await _prdDbReposService.MpNotificadoRepo.SaveAllUnapprMpNotficado(unapprovedMpNoticados);
                await _prdDbReposService.PtNotificadoRepo.SaveAllUnapprPtNotficado(unapprovedPtNoticados);
                await _prdDbReposService.OrdenProduccionMpRepo.SaveAllOrdenesProduccionPmAsync(ordenesProduccionPm);
                await _prdDbReposService.OrdenProduccionPtRepo.SaveAllOrdenesProduccionPtAsync(ordenesProduccionPt);
                await _prdDbReposService.OrdenProduccionRepo.SaveAllOrdenesProduccionAsync(ordenesProduccion);

                await _appDbReposService.UsuarioRepo.SaveAllUsuariosAsync(usuarios);
                await _appDbReposService.PeriodoRepo.SaveRangosPeriodosAsync(rangoPeriodos);
                await _appDbReposService.EmpleadosRepo.SaveAllEmpleadosAsync(empleados);



                InitData();

            }
            catch (Exception ex)
            {
                await _dialogService.ShowToast(ex.Message);
            }
            finally
            {
                await _dialogService.HideLoadingPopUpAsync();
            }


        }


        public async void InitData()
        {
            SharedData.AllOrdenesProduccionGroups = await GetAllOrdenesProduccionAsync();
            if (SharedData.AllOrdenesProduccionGroups.Count > 0)
            {
                OrdenesProdCount = SharedData.AllOrdenesProduccionGroups.Count;
                PendingOrdenesProdCount = SharedData.AllOrdenesProduccionGroups
                    .SelectMany(opg => opg)
                    .Count(op => op.Saldo > 0);
                var ordenesProduccionMp = await GetAllOrdenesProduccionMpAsync();
                OrdenesProdMpCount = ordenesProduccionMp
                    .Select(opg => opg)
                    .Count(op => op.Saldo > 0);

                var allPtNotificados = await _prdDbReposService.PtNotificadoRepo.GetAllPtNotificadosAsync();
                var allMpNotificados = await _prdDbReposService.MpNotificadoRepo.GetAllMpNotificadosAsync();

                // Informe de PT
                PtNotificadoDesyncCount = allPtNotificados is not null ? allPtNotificados.Count(pt => pt.Sincronizado == false) : 0;
                UnapprovPtNotificadoPrdCount = allPtNotificados is not null ? allPtNotificados.Count(pt =>
                    pt.Sincronizado == true &&
                    pt.AprobarAutoProduccion == false &&
                    pt.AprobarAutoInventario == false
                    ) : 0;
                UnapprovPtNotificadoInvCount = allPtNotificados is not null ? allPtNotificados.Count(pt =>
                    pt.Sincronizado == true &&
                    pt.AprobarAutoProduccion == true &&
                    pt.AprobarAutoInventario == false
                    ) : 0;

                //Informes de MP
                MpNotificadoDesyncCount = allMpNotificados is not null ? allMpNotificados.Count(pt => pt.Sincronizado == false) : 0;
                UnapprovMpNotificadoPrdCount = allMpNotificados is not null ? allMpNotificados.Count(pt =>
                    pt.Sincronizado == true &&
                    pt.AprobarAutoProduccion == false &&
                    pt.AprobarAutoInventario == false
                    ) : 0;
                UnapprovMpNotificadoInvCount = allMpNotificados is not null ? allMpNotificados.Count(pt =>
                    pt.Sincronizado == true &&
                    pt.AprobarAutoProduccion == true &&
                    pt.AprobarAutoInventario == false
                    ) : 0;


                return;
            }
        }


        internal async Task BackButtonPressed()
        {
            var res = await _dialogService.DisplayAlert("Salir", "¿Desea cerrar sesión?", "Aceptar", "Cancelar");
            if (res)
            {
                await NavigationService.LogOutAsync();
            }
        }

        private async Task<ObservableCollection<OrdenProduccionGroup>> GetAllOrdenesProduccionAsync()
        {
            try
            {
                var ordenesProduccionDb = await _prdDbReposService.OrdenProduccionRepo.GetAllOrdenesProduccionAsync();

                if (ordenesProduccionDb.Count == 0)
                {
                    return [];
                }
                else
                {
                    var ordenesProduccionPt = await _prdDbReposService.OrdenProduccionPtRepo.GetAllOrdenesProduccionPt();
                    return MapOrdenesProduccionGrouped(ordenesProduccionDb, ordenesProduccionPt);
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowToast(ex.Message, ToastDuration.Long);
            }
            return null;
        }

        private async Task<ObservableCollection<OrdenProduccionMp>> GetAllOrdenesProduccionMpAsync()
        {
            var ordenesProduccionMpDb = await _prdDbReposService.OrdenProduccionMpRepo.GetAllOrdenesProduccionMpAsync();
            if (ordenesProduccionMpDb.Count == 0)
            {
                return [];
            }
            return ordenesProduccionMpDb;
        }


        private ObservableCollection<OrdenProduccionGroup> MapOrdenesProduccionGrouped(ObservableCollection<OrdenProduccion> ordenesPrd, ObservableCollection<OrdenProduccionPt> ordenesProducciondPt)
        {
            var gruposPt = ordenesProducciondPt
                .GroupBy(pt => (pt.Centro, pt.CodigoProduccion, pt.Orden))
                .ToDictionary(
                    g => g.Key,
                    g => g.AsEnumerable(),
                    EqualityComparer<(string, string, int)>.Default);

            return new ObservableCollection<OrdenProduccionGroup>(ordenesPrd.Select(orden =>
                    new OrdenProduccionGroup
                    (
                        orden,
                        gruposPt.TryGetValue((orden.Centro, orden.CodigoProduccion, orden.Orden), out var grupoPt)
                            ? grupoPt.ToList()
                            : new List<OrdenProduccionPt>()
                    )
                )
            );
        }



    }
}
