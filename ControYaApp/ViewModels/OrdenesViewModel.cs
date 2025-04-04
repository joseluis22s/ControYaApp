using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
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
    [QueryProperty(nameof(EsNotificado), "esNotificado")]
    [QueryProperty(nameof(OrdenProduccionPt), "ordenProduccionPt")]
    public partial class OrdenesViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly AppDbReposService _appDbReposService;
        private readonly PrdDbReposService _prdDbReposService;


        public ISharedData SharedData { get; set; }


        private readonly OrdenProduccionFilter _ordenProduccionFilter;

        public OrdenProduccionPt OrdenProduccionPt { get; set; }

        public bool EsNotificado { get; set; }



        private readonly AppShellViewModel _appShellViewModel;

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


        public OrdenesViewModel(INavigationService navigationService, IDialogService dialogService,
            AppDbReposService appDbReposService, PrdDbReposService prdDbReposService,
            RestService restService, ISharedData sharedData,
                                OrdenProduccionFilter ordenProduccionFilter, AppShellViewModel appShellViewModel) : base(navigationService)
        {
            _dialogService = dialogService;
            _appDbReposService = appDbReposService;
            _prdDbReposService = prdDbReposService;


            SharedData = sharedData;


            _ordenProduccionFilter = ordenProduccionFilter;
            _appShellViewModel = appShellViewModel;


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
                var ordenesProduccionMpDb = await _prdDbReposService.OrdenProduccionMpRepo.GetOrdenesProduccionMpByOrdenProduccion(ordenProduccion.OrdenProduccion);
                var ordenesProduccionMpSourceDb = await _prdDbReposService.OrdenProduccionMpRepo.GetOrdenesProduccionMpByOrdenProduccion(ordenProduccion.OrdenProduccion);
                if (ordenesProduccionMpDb is null)
                {
                    await _dialogService.ShowToast("Problemas al recuperar datos.", ToastDuration.Long);
                    return;
                }
                if (ordenesProduccionMpDb.Count == 0)
                {
                    await _dialogService.ShowToast("Usuario sin materiales de producción asignados para esta orden de producción.", ToastDuration.Long);
                    return;
                }

                List<OrdenProduccionMaterialGroup> ordenesProduccionMaterialGroup = new(MapOrdenesProduccionMaterialGrouped(ordenesProduccionMpDb));
                var ordenesProduccionMaterialGroupSource = MapOrdenesProduccionMaterialGrouped(ordenesProduccionMpSourceDb);
                var empleados = await _appDbReposService.EmpleadosRepo.GetAllEmpleadosAsync();
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
                    var notificadoValue = await _prdDbReposService.OrdenProduccionPtRepo.GetNotificadoValue(OrdenProduccionPt);

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
            }
        }


        public async Task GetOrdenesProduccionAsync()
        {
            try
            {
                if (SharedData.AllOrdenesProduccionGroups.Count == 0)
                {
                    await _dialogService.ShowToast("No se han encontrado ordenes de producción");
                    return;
                }
                OrdenesProduccionGroups = SharedData.AllOrdenesProduccionGroups;
                OrdenesGroupLoaded = true;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowToast(ex.Message, ToastDuration.Long);
            }

        }

        private async Task FilterOrdenes()
        {
            List<OrdenProduccionGroup> allOrdenesProduccionGroups = new(SharedData.AllOrdenesProduccionGroups);
            string action = await _dialogService.DisplayActionSheet("Filtrar ordenes de producción:", "Cancelar", null, "Todas", "Con saldo pendiente", "Sin saldo pendiente");
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
                var empleados = await _appDbReposService.EmpleadosRepo.GetAllEmpleadosAsync();

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
            return await _appDbReposService.PeriodoRepo.GetRangosPeriodosAsync();
        }

        internal async Task BackButtonPressed()
        {
            var res = await _dialogService.DisplayAlert("Salir", "¿Desea cerrar sesión?", "Aceptar", "Cancelar");
            if (res)
            {
                await NavigationService.LogOutAsync();
            }
        }


    }
}
