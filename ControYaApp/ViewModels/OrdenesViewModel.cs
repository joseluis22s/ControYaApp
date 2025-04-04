using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ControYaApp.Models;
using ControYaApp.Services.Dialog;
using ControYaApp.Services.LocalDatabase.Repositories;
using ControYaApp.Services.Navigation;
using ControYaApp.Services.OrdenProduccionFilter;
using ControYaApp.Services.SharedData;
using ControYaApp.Services.WebService;
using ControYaApp.ViewModels.Base;

namespace ControYaApp.ViewModels
{
    [QueryProperty(nameof(EsNotificado), "esNotificado")]
    [QueryProperty(nameof(OrdenProduccionPt), "ordenProduccionPt")]
    public partial class OrdenesViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;
        public ISharedData SharedData { get; set; }


        private readonly OrdenProduccionFilter _ordenProduccionFilter;

        public OrdenProduccionPt OrdenProduccionPt { get; set; }

        public bool EsNotificado { get; set; }

        private readonly OrdenProduccionPtRepo _ordenProduccionPtRepo;
        private readonly PeriodoRepo _periodoRepo;

        private readonly EmpleadosRepo _empleadosRepo;

        private readonly OrdenProduccionMpRepo _ordenProduccionMpRepo;

        private readonly AppShellViewModel _appShellViewModel;

        private readonly HomeViewModel _homeViewModel;

        public Periodos RangoPeriodos { get; set; }



        private OrdenProduccionPt _ordenProduccionPtSelected;
        public OrdenProduccionPt OrdenProduccionPtSelected
        {
            get => _ordenProduccionPtSelected;
            set => SetProperty(ref _ordenProduccionPtSelected, value);
        }


        private ObservableCollection<OrdenProduccionGroup> _ordenesProduccionGroups;
        public ObservableCollection<OrdenProduccionGroup> OrdenesProduccionGroups
        {
            get => _ordenesProduccionGroups;

            set => SetProperty(ref _ordenesProduccionGroups, value);
        }


        private bool _ordenesGroupLoaded;
        public bool OrdenesGroupLoaded
        {
            get => _ordenesGroupLoaded;
            set => SetProperty(ref _ordenesGroupLoaded, value);
        }
        public string Comer { get; set; }


        private bool _ordenesGroupIsNull;
        public bool OrdenesGroupIsNull
        {
            get => _ordenesGroupIsNull = !_ordenesGroupLoaded;
            set => SetProperty(ref _ordenesGroupIsNull, value);
        }



        public ICommand GetOrdenesCommand { get; }

        public ICommand NotificarPtCommand { get; }

        public ICommand FilterOrdenesCommand { get; }

        public ICommand SincronizarOrdenesProduccionCommand { get; }

        public ICommand NotificarPmCommand { get; }


        public OrdenesViewModel(INavigationService navigationService, IDialogService dialogService, RestService restService, OrdenProduccionPtRepo ordenProduccionPtRepo, EmpleadosRepo empleadosRepo,
                                ISharedData sharedData, AppShellViewModel appShellViewModel, HomeViewModel homeViewModel,
                                OrdenProduccionMpRepo ordenProduccionMpRepo, OrdenProduccionFilter ordenProduccionFilter,
                                PeriodoRepo periodoRepo) : base(navigationService)
        {
            _dialogService = dialogService;
            SharedData = sharedData;

            _ordenProduccionMpRepo = ordenProduccionMpRepo;
            _ordenProduccionPtRepo = ordenProduccionPtRepo;
            _empleadosRepo = empleadosRepo;
            _periodoRepo = periodoRepo;

            _ordenProduccionFilter = ordenProduccionFilter;

            _appShellViewModel = appShellViewModel;
            _homeViewModel = homeViewModel;

            InitData();

            GetOrdenesCommand = new AsyncRelayCommand(GetOrdenesProduccionAsync);
            //NotificarPtCommand = new AsyncRelayCommand(NotificarPtAsync);
            //NotificarPtCommand = new AsyncRelayCommand(NotificarPtAsync);
            FilterOrdenesCommand = new AsyncRelayCommand(FilterOrdenes);
            SincronizarOrdenesProduccionCommand = new RelayCommand(SincronizarOrdenesProduccion);
            NotificarPmCommand = new AsyncRelayCommand<OrdenProduccionGroup>(NotificarPmAsync);

            NotificarPtCommand = new AsyncRelayCommand<OrdenProduccionPt>(NotificarPtAsync);
            VaciarOrdenes();
        }


        private async Task NotificarPmAsync(OrdenProduccionGroup ordenProduccion)
        {
            try
            {
                var ordenesProduccionMpDb = await _ordenProduccionMpRepo.GetOrdenesProduccionMpByOrdenProduccion(ordenProduccion.OrdenProduccion);
                var ordenesProduccionMpSourceDb = await _ordenProduccionMpRepo.GetOrdenesProduccionMpByOrdenProduccion(ordenProduccion.OrdenProduccion);
                if (ordenesProduccionMpDb is null)
                {
                    await _dialogService.ShowToast("Problemas al recuperar datos.", ToastDuration.Long);
                    //TODO: Eliminar -> await Toast.Make("Problemas al recuperar datos.", ToastDuration.Long).Show();
                    return;
                }
                if (ordenesProduccionMpDb.Count == 0)
                {
                    await _dialogService.ShowToast("Usuario sin materiales de producción asignados para esta orden de producción.", ToastDuration.Long);
                    //TODO: Eliminar -> await Toast.Make("Usuario sin materiales de producción asignados para esta orden de producción.", ToastDuration.Long).Show();
                    return;
                }

                List<OrdenProduccionMaterialGroup> ordenesProduccionMaterialGroup = new(MapOrdenesProduccionMaterialGrouped(ordenesProduccionMpDb));
                var ordenesProduccionMaterialGroupSource = MapOrdenesProduccionMaterialGrouped(ordenesProduccionMpSourceDb);
                var empleados = await _empleadosRepo.GetAllEmpleadosAsync();
                empleados = empleados.OrderBy(e => e.NombreEmpleado).ToObservableCollection();

                var rangosPeriodos = await GetRangosPeriodosAsync();

                var navParameter = new ShellNavigationQueryParameters
                    {
                        { "ordenesProdMpGrouped", ordenesProduccionMaterialGroup},
                        { "ordenesProdMpGroupedSource", ordenesProduccionMaterialGroupSource},
                        { "empleados", empleados},
                        { "rangosPeriodos", rangosPeriodos}
                    };
                await NavigationService.GoToAsync("notificarPm", navParameter);

            }
            catch (Exception ex)
            {
                await _dialogService.ShowToast(ex.Message, ToastDuration.Long);
                //TODO: Eliminar -> await Toast.Make(ex.Message, ToastDuration.Long).Show();
            }
        }

        private async void InitData()
        {
            try
            {
                if (SharedData.AllOrdenesProduccionGroups.Count != 0)
                {
                    OrdenesProduccionGroups = _ordenProduccionFilter.FilteredOrdenesProduccionGroup(OrdenProduccionFilter.OrdenesProduccionFilters.PendingSaldo, SharedData.AllOrdenesProduccionGroups.ToList());
                    OrdenesGroupLoaded = true;
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowToast(ex.Message, ToastDuration.Long);
                //TODO: Eliminar -> await Toast.Make(ex.Message).Show();
            }
        }


        private void SincronizarOrdenesProduccion()
        {
            _appShellViewModel.GetAndSaveDataCommand.Execute(null);
            // TODO: Averiguar por que se debe llamar a este metodo aqui
            //_homeViewModel.InitData();
            if (SharedData.AllOrdenesProduccionGroups.Count != 0)
            {
                OrdenesGroupLoaded = true;
                return;
            }
            OrdenesGroupLoaded = false;
            return;

        }


        private void VaciarOrdenes()
        {
            WeakReferenceMessenger.Default.Register<ClearDataMessage>(this, (r, m) =>
            {
                if (m.Value == "Vaciar")
                {
                    OrdenesProduccionGroups?.Clear();
                }
            });
        }


        internal async void Appearing()
        {
            try
            {
                if (EsNotificado)
                {
                    var notificadoValue = await _ordenProduccionPtRepo.GetNotificadoValue(OrdenProduccionPt);

                    // TODO: Verificar si el valor de notificado cambia. Creo que falta en la propiedad notificado de OrdenProduccionPt
                    //       o directamente usar notificapropertuychanged aqui.
                    OrdenesProduccionGroups.FirstOrDefault(opg =>
                            opg.OrdenProduccion.Centro == OrdenProduccionPt.Centro &&
                            opg.OrdenProduccion.CodigoProduccion == OrdenProduccionPt.CodigoProduccion &&
                            opg.OrdenProduccion.Orden == OrdenProduccionPt.Orden
                        ).FirstOrDefault(oppt =>
                            oppt.CodigoProducto == OrdenProduccionPt.CodigoProducto &&
                            oppt.CodigoMaterial == OrdenProduccionPt.CodigoMaterial
                        ).Notificado = notificadoValue;

                    SharedData.AllOrdenesProduccionGroups.FirstOrDefault(opg =>
                            opg.OrdenProduccion.Centro == OrdenProduccionPt.Centro &&
                            opg.OrdenProduccion.CodigoProduccion == OrdenProduccionPt.CodigoProduccion &&
                            opg.OrdenProduccion.Orden == OrdenProduccionPt.Orden
                        ).FirstOrDefault(oppt =>
                            oppt.CodigoProducto == OrdenProduccionPt.CodigoProducto &&
                            oppt.CodigoMaterial == OrdenProduccionPt.CodigoMaterial
                        ).Notificado = notificadoValue;
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowToast(ex.Message, ToastDuration.Long);
                //TODO: Eliminar -> await Toast.Make(ex.Message, ToastDuration.Long).Show();
            }
        }


        public async Task GetOrdenesProduccionAsync()
        {
            try
            {
                if (SharedData.AllOrdenesProduccionGroups.Count == 0)
                {
                    await _dialogService.ShowToast("No se han encontrado ordenes de producción");
                    //TODO: Eliminar -> await Toast.Make("No se han encontrado ordenes de producción").Show();
                    return;
                }
                OrdenesProduccionGroups = SharedData.AllOrdenesProduccionGroups;
                OrdenesGroupLoaded = true;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowToast(ex.Message, ToastDuration.Long);
                //TODO: Eliminar -> await Toast.Make(ex.Message).Show();
            }

        }

        private async Task FilterOrdenes()
        {
            List<OrdenProduccionGroup> allOrdenesProduccionGroups = new(SharedData.AllOrdenesProduccionGroups);
            string action = await Shell.Current.DisplayActionSheet("Filtrar ordenes de producción:", "Cancelar", null, "Todas", "Con saldo pendiente", "Sin saldo pendiente");
            if (action == "Con saldo pendiente")
            {
                OrdenesProduccionGroups?.Clear();
                OrdenesProduccionGroups = _ordenProduccionFilter.FilteredOrdenesProduccionGroup(OrdenProduccionFilter.OrdenesProduccionFilters.PendingSaldo, allOrdenesProduccionGroups);
                return;
            }
            if (action == "Sin saldo pendiente")
            {
                OrdenesProduccionGroups?.Clear();
                OrdenesProduccionGroups = _ordenProduccionFilter.FilteredOrdenesProduccionGroup(OrdenProduccionFilter.OrdenesProduccionFilters.NoPendingSaldo, allOrdenesProduccionGroups);
                return;
            }

            OrdenesProduccionGroups?.Clear();
            OrdenesProduccionGroups = new(allOrdenesProduccionGroups);
        }

        public async Task NotificarPtAsync(OrdenProduccionPt ordenProduccionPt)
        {
            try
            {
                var empleados = await _empleadosRepo.GetAllEmpleadosAsync();

                empleados = empleados.OrderBy(e => e.NombreEmpleado).ToObservableCollection();

                var navParameter = new ShellNavigationQueryParameters
                {
                    { "ordenProduccionPt", ordenProduccionPt},
                    { "empleados", empleados}
                };

                await NavigationService.GoToAsync("notificarPt", navParameter);

            }
            catch (Exception ex)
            {
                await _dialogService.ShowToast(ex.Message, ToastDuration.Long);
                //TODO: Eliminar -> await Toast.Make(ex.Message).Show();
            }
        }


        private ObservableCollection<OrdenProduccionMaterialGroup> MapOrdenesProduccionMaterialGrouped(ObservableCollection<OrdenProduccionMp> ordenesProducciondMp)
        {
            // Agrupar los OrdenProduccionMp por CodigoProduccion, Orden, CodigoMaterial y Material
            var grupos = ordenesProducciondMp
                .GroupBy(mp => new
                {
                    mp.CodigoProduccion,
                    mp.Orden,
                    mp.CodigoMaterial,
                    mp.Material
                })
                .Select(g => new OrdenProduccionMaterialGroup(
                    g.Key.CodigoProduccion, // CodigoProduccion del grupo
                    g.Key.Orden,           // Orden del grupo
                    g.Key.CodigoMaterial,   // CodigoMaterial del grupo
                    g.Key.Material,         // Material del grupo
                    g.ToList()              // Lista de OrdenProduccionMp del grupo
                ))
                .ToList();

            // Retornar la colección observable
            return new ObservableCollection<OrdenProduccionMaterialGroup>(grupos);
        }

        private async Task<Periodos> GetRangosPeriodosAsync()
        {
            return await _periodoRepo.GetRangosPeriodosAsync();
        }

        internal async Task BackButtonPressed()
        {
            var res = await Shell.Current.DisplayAlert("Salir", "¿Desea cerrar sesión?", "Aceptar", "Cancelar");
            if (res)
            {
                await NavigationService.LogOutAsync();
            }
        }


    }
}
