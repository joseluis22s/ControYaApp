using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ControYaApp.Models;
using ControYaApp.Services.LocalDatabase.Repositories;
using ControYaApp.Services.OrdenProduccionFilter;
using ControYaApp.Services.SharedData;
using ControYaApp.Services.WebService;

namespace ControYaApp.ViewModels
{
    [QueryProperty(nameof(EsNotificado), "esNotificado")]
    [QueryProperty(nameof(OrdenProduccionPt), "ordenProduccionPt")]
    public partial class OrdenesViewModel : BaseViewModel
    {
        public ISharedData SharedData { get; set; }


        private ObservableCollection<OrdenProduccionGroup> _allOrdenesGrouped;



        private OrdenProduccionFilter OrdenProduccionFilter { get; } = new();

        public OrdenProduccionPt OrdenProduccionPt { get; set; }

        public bool EsNotificado { get; set; }





        private readonly OrdenProduccionRepo _ordenProduccionRepo;

        private readonly OrdenProduccionPtRepo _ordenProduccionPtRepo;

        private readonly EmpleadosRepo _empleadosRepo;

        private readonly AppShellViewModel _appShellViewModel;

        private readonly HomeViewModel _homeViewModel;



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



        public OrdenesViewModel(RestService restService, OrdenProduccionRepo ordenProduccionRepo, OrdenProduccionPtRepo ordenProduccionPtRepo, EmpleadosRepo empleadosRepo, ISharedData sharedData, AppShellViewModel appShellViewModel, HomeViewModel homeViewModel)
        {

            SharedData = sharedData;


            _ordenProduccionRepo = ordenProduccionRepo;
            _ordenProduccionPtRepo = ordenProduccionPtRepo;
            _empleadosRepo = empleadosRepo;
            _appShellViewModel = appShellViewModel;
            _homeViewModel = homeViewModel;

            InitData();

            GetOrdenesCommand = new AsyncRelayCommand(GetOrdenesProduccionAsync);
            NotificarPtCommand = new AsyncRelayCommand(NotificarPtAsync);
            FilterOrdenesCommand = new AsyncRelayCommand(() => FilterOrdenes(SharedData.AllOrdenesProduccionGroups));
            SincronizarOrdenesProduccionCommand = new RelayCommand(SincronizarOrdenesProduccion);

            VaciarOrdenes();
        }

        private async void InitData()
        {
            try
            {
                if (SharedData.AllOrdenesProduccionGroups.Count != 0)
                {
                    OrdenesProduccionGroups = OrdenProduccionFilter.FilteredOrdenesProduccionGroup(OrdenProduccionFilter.OrdenesProduccionFilters.Pending, SharedData.AllOrdenesProduccionGroups);
                    OrdenesGroupLoaded = true;
                }
            }
            catch (Exception ex)
            {
                await Toast.Make(ex.Message).Show();
            }
        }

        private void SincronizarOrdenesProduccion()
        {
            _appShellViewModel.ExtraerDatosCommand.Execute(null);
            _homeViewModel.InitData();
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
                await Toast.Make(ex.Message, ToastDuration.Long).Show();
            }
        }


        public async Task GetOrdenesProduccionAsync()
        {
            try
            {
                if (SharedData.AllOrdenesProduccionGroups.Count == 0)
                {
                    await Toast.Make("No se han encontrado ordenes de producción").Show();
                    return;
                }
                OrdenesProduccionGroups = SharedData.AllOrdenesProduccionGroups;
                OrdenesGroupLoaded = true;
            }
            catch (Exception ex)
            {
                await Toast.Make(ex.Message).Show();
            }
            //var loadingPopUpp = new LoadingPopUp();
            //_ = Shell.Current.CurrentPage.ShowPopupAsync(loadingPopUpp);

            //try
            //{
            //    var ordenesProduccionDb = await _ordenProduccionRepo.GetOrdenesProduccionByUsuarioSistema(SharedData.UsuarioSistema);

            //    if (ordenesProduccionDb.Count != 0)
            //    {
            //        var ordenesProduccionPt = await _ordenProduccionPtRepo.GetAllOrdenesProduccionPt();
            //        _allOrdenesGrouped = MapOrdenesProduccionGrouped(ordenesProduccionDb, ordenesProduccionPt);
            //        OrdenesProduccionGroups = FilteredOrdenesProduccionGroup(OrdenesProduccionFilters.Pending, _allOrdenesGrouped);
            //        OrdenesGroupLoaded = true;
            //    }
            //    else
            //    {
            //        await Toast.Make("No se han encontrado datos", ToastDuration.Long).Show();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    await Toast.Make(ex.Message, ToastDuration.Long).Show();
            //}
            //finally
            //{
            //    await loadingPopUpp.CloseAsync();
            //}

        }

        private async Task FilterOrdenes(ObservableCollection<OrdenProduccionGroup> allOrdenesGrouped)
        {
            string action = await Shell.Current.DisplayActionSheet("Filtrar ordenes de producción:", "Cancelar", null, "Todas", "Pendientes", "Notificadas");
            if (action == "Pendientes")
            {
                OrdenesProduccionGroups = OrdenProduccionFilter.FilteredOrdenesProduccionGroup(OrdenProduccionFilter.OrdenesProduccionFilters.Pending, allOrdenesGrouped);
                return;
            }
            if (action == "Notificadas")
            {
                OrdenesProduccionGroups = OrdenProduccionFilter.FilteredOrdenesProduccionGroup(OrdenProduccionFilter.OrdenesProduccionFilters.Notified, allOrdenesGrouped);
                return;
            }
            OrdenesProduccionGroups = allOrdenesGrouped;
        }

        //private ObservableCollection<OrdenProduccionGroup> FilteredOrdenesProduccionGroup(OrdenesProduccionFilters filter, ObservableCollection<OrdenProduccionGroup> ordenesProduccionGroups)
        //{
        //    if (OrdenesProduccionFilters.Notified == filter)
        //    {
        //        return ordenesProduccionGroups
        //            .Where(opg => opg.All(oppt => oppt.Notificado == 0)).ToObservableCollection();

        //    }
        //    if (OrdenesProduccionFilters.Pending == filter)
        //    {
        //        return ordenesProduccionGroups
        //            .Where(opg => opg.All(oppt => oppt.Saldo != 0)).ToObservableCollection(); ;
        //    }

        //    return ordenesProduccionGroups;
        //}


        public async Task NotificarPtAsync()
        {
            try
            {
                if (OrdenProduccionPtSelected.Saldo == 0)
                {
                    await Toast.Make("Esta orden no tiene saldo para notificar", ToastDuration.Long).Show();
                    return;
                }
                var empleados = await _empleadosRepo.GetAllEmpleadosAsync();

                empleados = empleados.OrderBy(e => e.NombreEmpleado).ToObservableCollection();


                var navParameter = new ShellNavigationQueryParameters
                {
                    { "ordenProduccionPt", OrdenProduccionPtSelected},
                    { "empleados", empleados}
                };
                await Shell.Current.GoToAsync("notificarPt", navParameter);
            }
            catch (Exception ex)
            {
                await Toast.Make(ex.Message).Show();
            }
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


        internal async Task BackButtonPressed()
        {
            var res = await Shell.Current.DisplayAlert("Salir", "¿Desea cerrar sesión?", "Aceptar", "Cancelar");
            if (res)
            {
                await Shell.Current.GoToAsync("//login");
            }
        }


    }
}
