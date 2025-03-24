using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using ControYaApp.Models;
using ControYaApp.Services.DI;
using ControYaApp.Services.LocalDatabase.Repositories;
using ControYaApp.Services.OrdenProduccionFilter;
using ControYaApp.Services.SharedData;
using ControYaApp.Services.WebService;
using ControYaApp.Views.Controls;

namespace ControYaApp.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private readonly OrdenProduccionRepo _ordenProduccionRepo;

        private readonly OrdenProduccionPtRepo _ordenProduccionPtRepo;

        private readonly LocalRepoService _localRepoService;

        private readonly RestService _restService;



        private OrdenProduccionFilter OrdenProduccionFilter { get; set; }

        public ISharedData SharedData { get; set; }



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

        public ICommand GoToOrdenesCommand { get; }


        public HomeViewModel(ISharedData sharedData, OrdenProduccionRepo ordenProduccionRepo, OrdenProduccionPtRepo ordenProduccionPtRepo, LocalRepoService localRepoService, RestService restService)
        {
            SharedData = sharedData;

            _ordenProduccionRepo = ordenProduccionRepo;
            _ordenProduccionPtRepo = ordenProduccionPtRepo;
            _restService = restService;
            _localRepoService = localRepoService;

            SincronizarOrdenesProduccionCommand = new AsyncRelayCommand(SincronizarOrdenesProduccionAsync);
            GoToOrdenesCommand = new AsyncRelayCommand(GoToOrdenes);

            InitData();
        }

        private async Task SincronizarOrdenesProduccionAsync()
        {

            var loadingPopUpp = new LoadingPopUp();
            _ = Shell.Current.CurrentPage.ShowPopupAsync(loadingPopUpp);

            try
            {
                //WeakReferenceMessenger.Default.Send(new ClearDataMessage("Vaciar"));
                var AllPtNotificados = await _localRepoService.PtNotificadoRepo.GetAllPtNotificadoAsync();
                var AllMpNotificados = await _localRepoService.MpNotificadoRepo.GetAllMpNotificadoAsync();

                // Cuando haya PT notficados (que AllPtNotificados no sea nullo y cuando no es nulo que al menos haya uno)
                // notifica los que esten con `NotificarManyPtAsync()` y luego los elimina con `DeleteAllPtNotificado()`.
                //
                // Lo mismo ocurre con MP notificados.
                if (AllPtNotificados is not null && AllPtNotificados.Count != 0)
                {
                    var req = new
                    {
                        ptNotificados = AllPtNotificados
                    };
                    await _restService.NotificarManyPtAsync(req);
                    await Toast.Make("Se ha enviado PT notificados locales", ToastDuration.Long).Show();
                    await _localRepoService.PtNotificadoRepo.DeleteAllPtNotificado();
                }

                if (AllMpNotificados is not null && AllMpNotificados.Count != 0)
                {
                    var req = new
                    {
                        mpNotificados = AllMpNotificados
                    };
                    await _restService.NotificarManyMpAsync(req);
                    await Toast.Make("Se ha enviado MP notificados locales", ToastDuration.Long).Show();
                    await _localRepoService.MpNotificadoRepo.DeleteAllMpNotificado();
                }


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



        private async Task GoToOrdenes()
        {
            await Shell.Current.GoToAsync("//ordenes");
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
                OrdenesProdMpCount = await GetCountPendingOrdenesProduccionMpPAsync();
                return;
            }
            OrdenesGroupLoaded = !(OrdenesGroupIsNull = true);
        }


        internal async Task BackButtonPressed()
        {
            var res = await Shell.Current.DisplayAlert("Salir", "¿Desea cerrar sesión?", "Aceptar", "Cancelar");
            if (res)
            {
                await Shell.Current.GoToAsync("//login");
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

        private async Task<int> GetCountPendingOrdenesProduccionMpPAsync()
        {
            var ordenesProduccionMpDb = await _localRepoService.OrdenProduccionMpRepo.GetAllOrdenesProduccionPendingAsync();
            if (ordenesProduccionMpDb.Count == 0)
            {
                return 0;
            }
            else
            {
                return ordenesProduccionMpDb.Count;
            }
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
            //var ordenProducciondPtDic = ordenesProducciondPt
            //    .GroupBy(d => new
            //    {
            //        d.Centro,
            //        d.CodigoProduccion,
            //        d.Orden
            //    }
            //    ).ToDictionary(g => g.Key, g => g.ToList());


            //var ordenesProduccionGrouped = ordenesPrd
            //    .Select(ordenProduccion =>
            //    {
            //        var key = new
            //        {
            //            ordenProduccion.Centro,
            //            ordenProduccion.CodigoProduccion,
            //            ordenProduccion.Orden
            //        };

            //        return new OrdenProduccionGroup(
            //            ordenProduccion,
            //            ordenProducciondPtDic.TryGetValue(key, out var ordenesProducciondPtGrouped) ? ordenesProducciondPtGrouped : new List<OrdenProduccionPt>()
            //        );
            //    })
            //    .ToList();

            //return new ObservableCollection<OrdenProduccionGroup>(ordenesProduccionGrouped);
        }

        public ObservableCollection<OrdenProduccionGroup> FilteredOrdenesProduccionGroup(OrdenProduccionFilter.OrdenesProduccionFilters filter, ObservableCollection<OrdenProduccionGroup> ordenesProduccionGroups)
        {
            if (OrdenProduccionFilter.OrdenesProduccionFilters.Notified == filter)
            {
                return ordenesProduccionGroups
                    .Where(opg => opg.All(oppt => oppt.Notificado == 0)).ToObservableCollection();

            }
            if (OrdenProduccionFilter.OrdenesProduccionFilters.Pending == filter)
            {
                return ordenesProduccionGroups
                    .Where(opg => opg.All(oppt => oppt.Saldo > 0)).ToObservableCollection(); ;
            }

            return ordenesProduccionGroups;
        }


    }
}
