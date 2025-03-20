using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Input;
using ControYaApp.Models;
using ControYaApp.Services.LocalDatabase.Repositories;
using ControYaApp.Services.OrdenProduccionFilter;
using ControYaApp.Services.SharedData;

namespace ControYaApp.ViewModels
{
    [QueryProperty(nameof(OrdenesProduccionMaterialGroupSource), "ordenesProdMpGrouped")]
    [QueryProperty(nameof(OrdenesProduccionMaterialGroup), "ordenesProdMpGrouped")]
    [QueryProperty(nameof(Empleados), "empleados")]
    public partial class NotificarPmViewModel : BaseViewModel
    {

        private readonly OrdenProduccionMpRepo _ordenProduccionMpRepo;


        private readonly PmNotificadoRepo _pmNotificadoRepo;

        public ISharedData SharedData { get; set; }



        //private ObservableCollection<OrdenProduccionMaterialGroup> _ordenesProduccionMaterialGroup;
        //public ObservableCollection<OrdenProduccionMaterialGroup> OrdenesProduccionMaterialGroup
        //{
        //    get => _ordenesProduccionMaterialGroup;
        //    set => SetProperty(ref _ordenesProduccionMaterialGroup, value);
        public ObservableCollection<OrdenProduccionMaterialGroup> OrdenesProduccionMaterialGroup { get; set; }


        private ObservableCollection<OrdenProduccionMaterialGroup> _ordenesProduccionMaterialGroupSource;
        public ObservableCollection<OrdenProduccionMaterialGroup> OrdenesProduccionMaterialGroupSource
        {
            get => _ordenesProduccionMaterialGroupSource;
            set => SetProperty(ref _ordenesProduccionMaterialGroupSource, value);
        }


        private DateTime _fechaActual = DateTime.Now;
        public DateTime FechaActual
        {
            get => _fechaActual;
            set => SetProperty(ref _fechaActual, value);
        }


        private EmpleadoSistema? _empleadoSelected;
        public EmpleadoSistema? EmpleadoSelected
        {
            get => _empleadoSelected;
            set => SetProperty(ref _empleadoSelected, value);
        }


        private ObservableCollection<EmpleadoSistema>? _empleados;
        public ObservableCollection<EmpleadoSistema>? Empleados
        {
            get => _empleados;
            set => SetProperty(ref _empleados, value);
        }


        public ICommand GoBackCommand { get; }

        public ICommand NotificarPmCommand { get; }

        public ICommand FilterOrdenesProduccionMpCommand { get; }



        public NotificarPmViewModel(ISharedData sharedData, OrdenProduccionMpRepo ordenProduccionMpRepo, PmNotificadoRepo pmNotificadoRepo)
        {
            SharedData = sharedData;

            _ordenProduccionMpRepo = ordenProduccionMpRepo;
            _pmNotificadoRepo = pmNotificadoRepo;

            GoBackCommand = new AsyncRelayCommand(GoBackAsync);
            NotificarPmCommand = new AsyncRelayCommand(NotificarPm);
            FilterOrdenesProduccionMpCommand = new AsyncRelayCommand(() => FilterOrdenesProduccionMpAsync(OrdenesProduccionMaterialGroup));
        }


        private readonly OrdenProduccionMpFilter _ordenProduccionMpFilter;

        public NotificarPmViewModel(ISharedData sharedData, OrdenProduccionMpRepo ordenProduccionMpRepo, PmNotificadoRepo pmNotificadoRepo, OrdenProduccionMpFilter ordenProduccionMpFilter)
        {
            SharedData = sharedData;

            _ordenProduccionMpRepo = ordenProduccionMpRepo;
            _pmNotificadoRepo = pmNotificadoRepo;
            _ordenProduccionMpFilter = ordenProduccionMpFilter;

            GoBackCommand = new AsyncRelayCommand(GoBackAsync);
            NotificarPmCommand = new AsyncRelayCommand(NotificarPm);
        }

        private async Task FilterOrdenesProduccionMpAsync(ObservableCollection<OrdenProduccionMaterialGroup> allOrdenesMpGrouped)
        {
            if (OrdenesProduccionMaterialGroupSource is not null)
            {
                OrdenesProduccionMaterialGroupSource.Clear();
            }
            string action = await Shell.Current.DisplayActionSheet("Filtrar ordenes de producción:", "Cancelar", null, "Todas", "Pendientes");

            if (action == "Pendientes")
            {
                OrdenesProduccionMaterialGroupSource = _ordenProduccionMpFilter.FilteredOrdenesProduccionMpGroup(OrdenProduccionMpFilter.OrdenesProduccionMpFilters.Pending, allOrdenesMpGrouped);
                return;
            }

            OrdenesProduccionMaterialGroupSource = allOrdenesMpGrouped;
        }


        private async Task NotificarPm()
        {
            try
            {
                // Verifica si hay al menos un ítem seleccionado
                int selectedItemsCount = OrdenesProduccionMaterialGroupSource
                .SelectMany(opmg => opmg.Where(opm => opm.IsSelected == true))
                .ToList().Count;

                if (selectedItemsCount == 0)
                {
                    await Toast.Make("Ningún item seleccionado.", ToastDuration.Long).Show();
                    return;
                }

                // Obtén los ítems seleccionados de OrdenesProduccionMaterialGroupSource
                List<OrdenProduccionMp> selectedItemsSource = MapOrdenProduccionMpSelected(OrdenesProduccionMaterialGroupSource, OrdenesProduccionMaterialGroup);

                var pmNotificados = MapPmNotificado(selectedItemsSource, SharedData.AuthorizedNotification, FechaActual,
                                                    EmpleadoSelected.CodigoEmpleado, SharedData.UsuarioSistema);

                int updatedCount = await _ordenProduccionMpRepo.UpdateAllNotificadoAsync(selectedItemsSource);

                int savedOrdUpdateCount = 0;
                foreach (var pmNotificado in pmNotificados)
                {
                    savedOrdUpdateCount += await _pmNotificadoRepo.SaveOrUpdatePtNotificadoAsync(pmNotificado);
                }

                await Toast.Make($"{updatedCount} items actualizados y {savedOrdUpdateCount} notificados.", ToastDuration.Long).Show();

            }
            catch (Exception ex)
            {
                await Toast.Make(ex.Message, ToastDuration.Long).Show();
            }
        }

        private List<OrdenProduccionMp> MapOrdenProduccionMpSelected(ObservableCollection<OrdenProduccionMaterialGroup> selectedItemsGroupsSource, ObservableCollection<OrdenProduccionMaterialGroup> selectedItemsGroups)
        {
            var selectedItemsSource = selectedItemsGroupsSource
                .SelectMany(opmg => opmg.Where(opm => opm.IsSelected == true))
                .ToList();

            var selectedItemsOriginal = selectedItemsGroups
                .SelectMany(opmg => opmg
                    .Where(opm => selectedItemsSource
                        .Any(itemSource => itemSource.Id == opm.Id)))
                .ToList();

            for (int i = 0; i < selectedItemsSource.Count; i++)
            {
                var itemSource = selectedItemsSource[i];
                var itemOriginal = selectedItemsOriginal
                    .FirstOrDefault(opm => opm.Id == itemSource.Id);

                if (itemOriginal != null)
                {
                    // Suma el valor de "Notificado" del ítem original al ítem de OrdenesProduccionMaterialGroupSource
                    itemSource.Notificado += itemOriginal.Notificado;
                }
            }
            return selectedItemsSource;
        }

        private List<PmNotificado> MapPmNotificado(List<OrdenProduccionMp> ordenesProduccionMp, bool authorizedNotification,
                                                   DateTime fecha, string codigoEmpleado, string codigoUsuario)
        {
            return ordenesProduccionMp
                .Select(item => new PmNotificado
                {
                    Id = item.Id,                // Mapea el Id
                    Notificado = item.Notificado, // Mapea el Notificado (usa 0 si es null)
                    NotificacionAutorizada = authorizedNotification,
                    Fecha = fecha,
                    CodigoEmpleado = codigoEmpleado,
                    CodigoUsuario = codigoUsuario
                })
                .ToList();
        }

        private async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        //private async Task NotificarPm()
        //{
        //    // Verifica si hay al menos un ítem seleccionado
        //    int selectedItemsCount = OrdenesProduccionMaterialGroupSource
        //        .Count(opmg => opmg.Any(opm => opm.IsSelected == true));

        //    if (selectedItemsCount == 0)
        //    {
        //        await Toast.Make("Ningún item seleccionado.", ToastDuration.Long).Show();
        //        return;
        //    }

        //    // Obtén los ítems seleccionados de OrdenesProduccionMaterialGroupSource
        //    List<OrdenProduccionMp> selectedItemsSource = OrdenesProduccionMaterialGroupSource
        //        .SelectMany(opmg => opmg.Where(opm => opm.IsSelected == true))
        //        .ToList();

        //    // Obtén los ítems correspondientes de OrdenesProduccionMaterialGroup
        //    List<OrdenProduccionMp> selectedItemsOriginal = OrdenesProduccionMaterialGroup
        //        .SelectMany(opmg => opmg
        //            .Where(opm => selectedItemsSource
        //                .Any(itemSource => itemSource.Id == opm.Id)))
        //        .ToList();

        //    // Compara y actualiza el valor de "Notificado"
        //    for (int i = 0; i < selectedItemsSource.Count; i++)
        //    {
        //        var itemSource = selectedItemsSource[i];
        //        var itemOriginal = selectedItemsOriginal
        //            .FirstOrDefault(opm => opm.Id == itemSource.Id);

        //        if (itemOriginal != null)
        //        {
        //            // Suma el valor de "Notificado" del ítem original al ítem de OrdenesProduccionMaterialGroupSource
        //            itemSource.Notificado += itemOriginal.Notificado;
        //        }
        //    }

        //    var pmNotificados = MapPmNotificado(selectedItemsSource, SharedData.AuthorizedNotification, FechaActual,
        //                                        EmpleadoSelected.CodigoEmpleado, SharedData.UsuarioSistema);


        //}

    }
}
