using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
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
                PendingOrdenesProdCount = SharedData.AllOrdenesProduccionGroups.Count;
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
                var ordenesProduccionDb = await _ordenProduccionRepo.GetOrdenesProduccionByUsuarioSistema(SharedData.UsuarioSistema);

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
            return [];
        }

        private ObservableCollection<OrdenProduccionGroup> MapOrdenesProduccionGrouped(ObservableCollection<OrdenProduccion> ordenesPrd, ObservableCollection<OrdenProduccionPt> ordenesProducciondPt)
        {
            var ordenProducciondPtDic = ordenesProducciondPt
                .GroupBy(d => new
                {
                    d.Centro,
                    d.CodigoProduccion,
                    d.Orden
                }
                ).ToDictionary(g => g.Key, g => g.ToList());


            var ordenesProduccionGrouped = ordenesPrd
                .Select(ordenProduccion =>
                {
                    var key = new
                    {
                        ordenProduccion.Centro,
                        ordenProduccion.CodigoProduccion,
                        ordenProduccion.Orden
                    };

                    return new OrdenProduccionGroup(
                        ordenProduccion,
                        ordenProducciondPtDic.TryGetValue(key, out var ordenesProducciondPtGrouped) ? ordenesProducciondPtGrouped : new List<OrdenProduccionPt>()
                    );
                })
                .ToList();

            return new ObservableCollection<OrdenProduccionGroup>(ordenesProduccionGrouped);
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
