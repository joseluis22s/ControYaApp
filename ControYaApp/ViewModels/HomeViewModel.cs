using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using ControYaApp.Models;
using ControYaApp.Services.DI;
using ControYaApp.Services.LocalDatabase.Repositories;
using ControYaApp.Services.Navigation;
using ControYaApp.Services.OrdenProduccionFilter;
using ControYaApp.Services.SharedData;
using ControYaApp.Services.WebService;
using ControYaApp.ViewModels.Base;
using ControYaApp.Views.Controls;

namespace ControYaApp.ViewModels
{
    public partial class HomeViewModel : BaseViewModel
    {
        private readonly OrdenProduccionRepo _ordenProduccionRepo;

        private readonly OrdenProduccionPtRepo _ordenProduccionPtRepo;

        private readonly LocalRepoService _localRepoService;

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


        private bool _ordenesGroupLoaded;
        public bool OrdenesGroupLoaded
        {
            get => _ordenesGroupLoaded;
            set
            {
                SetProperty(ref _ordenesGroupLoaded, value);
            }
        }


        private bool _ordenesGroupIsNull;
        public bool OrdenesGroupIsNull
        {
            get => _ordenesGroupIsNull;
            set => SetProperty(ref _ordenesGroupIsNull, value);
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


        public HomeViewModel(INavigationService navigationService, ISharedData sharedData, OrdenProduccionRepo ordenProduccionRepo, OrdenProduccionPtRepo ordenProduccionPtRepo, LocalRepoService localRepoService, RestService restService) : base(navigationService)
        {
            SharedData = sharedData;

            _ordenProduccionRepo = ordenProduccionRepo;
            _ordenProduccionPtRepo = ordenProduccionPtRepo;
            _restService = restService;
            _localRepoService = localRepoService;

            SincronizarOrdenesProduccionCommand = new AsyncRelayCommand(SincronizarOrdenesProduccionAsync);

            InitData();
        }

        private async Task SincronizarOrdenesProduccionAsync()
        {
            NetworkAccess accessType = Connectivity.Current.NetworkAccess;

            if (accessType != NetworkAccess.Internet)
            {
                await Toast.Make("Sin conexión. No se puede realizar esta acción", ToastDuration.Long).Show();
                return;
            }

            var loadingPopUpp = new LoadingPopUp();
            _ = Shell.Current.CurrentPage.ShowPopupAsync(loadingPopUpp);

            try
            {
                //WeakReferenceMessenger.Default.Send(new ClearDataMessage("Vaciar"));
                var allPtNotificados = await _localRepoService.PtNotificadoRepo.GetAllPtNotificadoAsync();
                var allMpNotificados = await _localRepoService.MpNotificadoRepo.GetAllMpNotificadoAsync();

                // Cuando haya PT notficados (que AllPtNotificados no sea nullo y cuando no es nulo que al menos haya uno)
                // notifica los que esten con `NotificarManyPtAsync()` y luego los elimina con `DeleteAllPtNotificado()`.
                //
                // Lo mismo ocurre con MP notificados.
                if (allPtNotificados is not null && allPtNotificados.Count != 0)
                {
                    var ptNotificadosDesync = allPtNotificados.Where(pt => pt.Sincronizado == false).ToList();
                    foreach (var item in ptNotificadosDesync)
                    {
                        item.Sincronizado = true;
                    }
                    var req = new
                    {
                        ptNotificados = ptNotificadosDesync
                    };
                    await _restService.NotificarManyPtAsync(req);

                    await _localRepoService.PtNotificadoRepo.DeleteAllPtNotificado();
                }

                if (allMpNotificados is not null && allMpNotificados.Count != 0)
                {
                    var mpNotificadosDesync = allMpNotificados.Where(mp => mp.Sincronizado == false).ToList();
                    foreach (var item in mpNotificadosDesync)
                    {
                        item.Sincronizado = true;
                    }
                    var req = new
                    {
                        mpNotificados = mpNotificadosDesync
                    };
                    await _restService.NotificarManyMpAsync(req);

                    await _localRepoService.MpNotificadoRepo.DeleteAllMpNotificado();
                }


                var usuarios = await _restService.GetAllUsuariosAsync();
                var ordenesProduccion = await _restService.GetAllOrdenesProduccionAsync(SharedData.UsuarioSistema);
                var rangoPeriodos = await _restService.GetRangosPeriodos();
                var ordenesProduccionPt = await _restService.GetAllOrdenesProduccionPtAsync(SharedData.UsuarioSistema);
                var ordenesProduccionPm = await _restService.GetAllOrdenesProduccionPmAsync(SharedData.UsuarioSistema);
                var empleados = await _restService.GetAllEmpleadosAsync();
                var unapprovedPtNoticados = await _restService.GetUnapproveddPtPrdInv();
                var unapprovedMpNoticados = await _restService.GetUnapproveddMpPrdInv();

                await _localRepoService.MpNotificadoRepo.SaveAllUnapprMpNotficado(unapprovedMpNoticados);
                await _localRepoService.PtNotificadoRepo.SaveAllUnapprPtNotficado(unapprovedPtNoticados);
                await _localRepoService.EmpleadosRepo.SaveAllEmpleadosAsync(empleados);
                await _localRepoService.OrdenProduccionMpRepo.SaveAllOrdenesProduccionPmAsync(ordenesProduccionPm);
                await _localRepoService.OrdenProduccionPtRepo.SaveAllOrdenesProduccionPtAsync(ordenesProduccionPt);
                await _localRepoService.PeriodoRepo.SaveRangosPeriodosAsync(rangoPeriodos);
                await _localRepoService.OrdenesProduccionRepo.SaveAllOrdenesProduccionAsync(ordenesProduccion);
                await _localRepoService.UsuarioRepo.SaveAllUsuariosAsync(usuarios);



                InitData();

            }
            catch (Exception ex)
            {
                await Toast.Make(ex.Message).Show();
            }
            finally
            {
                await loadingPopUpp.CloseAsync();
            }


        }


        public async void InitData()
        {
            SharedData.AllOrdenesProduccionGroups = await GetAllOrdenesProduccionAsync();
            if (SharedData.AllOrdenesProduccionGroups.Count > 0)
            {
                OrdenesGroupLoaded = !(OrdenesGroupIsNull = false);
                OrdenesProdCount = SharedData.AllOrdenesProduccionGroups.Count;
                PendingOrdenesProdCount = SharedData.AllOrdenesProduccionGroups
                    .SelectMany(opg => opg)
                    .Count(op => op.Saldo > 0);
                var ordenesProduccionMp = await GetAllOrdenesProduccionMpAsync();
                OrdenesProdMpCount = ordenesProduccionMp
                    .Select(opg => opg)
                    .Count(op => op.Saldo > 0);

                var allPtNotificados = await _localRepoService.PtNotificadoRepo.GetAllPtNotificadoAsync();
                var allMpNotificados = await _localRepoService.MpNotificadoRepo.GetAllMpNotificadoAsync();

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
            OrdenesGroupLoaded = !(OrdenesGroupIsNull = true);
        }


        internal async Task BackButtonPressed()
        {
            var res = await Shell.Current.DisplayAlert("Salir", "¿Desea cerrar sesión?", "Aceptar", "Cancelar");
            if (res)
            {
                await NavigationService.LogOutAsync();
            }
        }

        private async Task<ObservableCollection<OrdenProduccionGroup>> GetAllOrdenesProduccionAsync()
        {
            try
            {
                var ordenesProduccionDb = await _ordenProduccionRepo.GetAllOrdenesProduccionAsync();

                if (ordenesProduccionDb.Count == 0)
                {
                    //await Toast.Make("No se han encontrado ordenes de producción", ToastDuration.Long).Show();
                    return [];
                }
                else
                {
                    var ordenesProduccionPt = await _ordenProduccionPtRepo.GetAllOrdenesProduccionPt();
                    return MapOrdenesProduccionGrouped(ordenesProduccionDb, ordenesProduccionPt);
                }
            }
            catch (Exception ex)
            {
                await Toast.Make(ex.Message, ToastDuration.Long).Show();
            }
            return null;
        }

        private async Task<ObservableCollection<OrdenProduccionMp>> GetAllOrdenesProduccionMpAsync()
        {
            var ordenesProduccionMpDb = await _localRepoService.OrdenProduccionMpRepo.GetAllOrdenesProduccionMpAsync();
            if (ordenesProduccionMpDb.Count == 0)
            {
                //await Toast.Make("No se han encontrado ordenes de producción", ToastDuration.Long).Show();
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
