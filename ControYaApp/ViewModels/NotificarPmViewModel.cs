﻿using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.Input;
using ControYaApp.Models;
using ControYaApp.Services.LocalDatabase.Repositories;
using ControYaApp.Services.OrdenProduccionFilter;
using ControYaApp.Services.SharedData;

namespace ControYaApp.ViewModels
{
    [QueryProperty(nameof(OrdenesProduccionMaterialGroupSource), "ordenesProdMpGroupedSource")]
    [QueryProperty(nameof(RangoPeriodos), "rangosPeriodos")]
    [QueryProperty(nameof(Empleados), "empleados")]
    public partial class NotificarPmViewModel : BaseViewModel
    {

        private readonly OrdenProduccionMpRepo _ordenProduccionMpRepo;


        private readonly MpNotificadoRepo _pmNotificadoRepo;

        public ISharedData SharedData { get; set; }



        //private ObservableCollection<OrdenProduccionMaterialGroup> _ordenesProduccionMaterialGroup;
        //public ObservableCollection<OrdenProduccionMaterialGroup> OrdenesProduccionMaterialGroup
        //{
        //    get => _ordenesProduccionMaterialGroup;
        //    set => SetProperty(ref _ordenesProduccionMaterialGroup, value);
        public List<OrdenProduccionMaterialGroup> OrdenesProduccionMaterialGroup { get; set; }


        private ObservableCollection<OrdenProduccionMaterialGroup> _ordenesProduccionMaterialGroupSource;
        public ObservableCollection<OrdenProduccionMaterialGroup> OrdenesProduccionMaterialGroupSource
        {
            get => _ordenesProduccionMaterialGroupSource;
            set
            {
                SetProperty(ref _ordenesProduccionMaterialGroupSource, value);

                foreach (var group in _ordenesProduccionMaterialGroupSource)
                {
                    foreach (var item in group)
                    {
                        item.Notificado = item.Saldo;
                    }
                }
            }
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

        private Periodos _rangoPeriodos;
        public Periodos RangoPeriodos
        {
            get => _rangoPeriodos;
            set => SetProperty(ref _rangoPeriodos, value);
        }


        public ICommand GoBackCommand { get; }

        public ICommand NotificarPmCommand { get; }

        public ICommand FilterOrdenesProduccionMpCommand { get; }

        public ICommand SeleccionarTodosCommand { get; }



        public NotificarPmViewModel(ISharedData sharedData, OrdenProduccionMpRepo ordenProduccionMpRepo, MpNotificadoRepo pmNotificadoRepo)
        {
            SharedData = sharedData;

            _ordenProduccionMpRepo = ordenProduccionMpRepo;
            _pmNotificadoRepo = pmNotificadoRepo;

            GoBackCommand = new AsyncRelayCommand(GoBackAsync);
            NotificarPmCommand = new AsyncRelayCommand(NotificarPm);
            FilterOrdenesProduccionMpCommand = new AsyncRelayCommand(() => FilterOrdenesProduccionMpAsync(OrdenesProduccionMaterialGroup));

        }


        private readonly OrdenProduccionMpFilter _ordenProduccionMpFilter;




        public NotificarPmViewModel(ISharedData sharedData, OrdenProduccionMpRepo ordenProduccionMpRepo, MpNotificadoRepo pmNotificadoRepo, OrdenProduccionMpFilter ordenProduccionMpFilter)
        {
            SharedData = sharedData;

            _ordenProduccionMpRepo = ordenProduccionMpRepo;
            _pmNotificadoRepo = pmNotificadoRepo;
            _ordenProduccionMpFilter = ordenProduccionMpFilter;

            GoBackCommand = new AsyncRelayCommand(GoBackAsync);
            NotificarPmCommand = new AsyncRelayCommand(NotificarPm);
            SeleccionarTodosCommand = new AsyncRelayCommand(SeleccionarTodosAsync);
        }

        private async Task SeleccionarTodosAsync()
        {
            if (OrdenesProduccionMaterialGroupSource is not null)
            {
                foreach (var itemGroup in OrdenesProduccionMaterialGroupSource)
                {
                    foreach (var item in itemGroup)
                    {
                        item.IsSelected = true;
                    }
                }
            }
        }

        private async Task FilterOrdenesProduccionMpAsync(List<OrdenProduccionMaterialGroup> allOrdenesMpGrouped)
        {
            if (OrdenesProduccionMaterialGroupSource is not null)
            {
                OrdenesProduccionMaterialGroupSource.Clear();
            }
            string action = await Shell.Current.DisplayActionSheet("Filtrar ordenes de producción:", "Cancelar", null, "Todas", "Pendientes");

            if (action == "Pendientes")
            {
                OrdenesProduccionMaterialGroupSource = _ordenProduccionMpFilter.FilteredOrdenesProduccionMpGroup(OrdenProduccionMpFilter.OrdenesProduccionMpFilters.Pending, allOrdenesMpGrouped.ToObservableCollection());
                return;
            }

            OrdenesProduccionMaterialGroupSource = allOrdenesMpGrouped.ToObservableCollection();
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

                if (EmpleadoSelected is null)
                {
                    await Toast.Make("Ningún empleado seleccionado.", ToastDuration.Long).Show();
                    return;
                }

                // Obtén los ítems seleccionados de OrdenesProduccionMaterialGroupSource
                //List<OrdenProduccionMp> selectedItemsSource = MapOrdenProduccionMpSelected(OrdenesProduccionMaterialGroupSource, OrdenesProduccionMaterialGroup);
                List<OrdenProduccionMp> selectedItemsSource = OrdenesProduccionMaterialGroupSource.SelectMany(g => g.Where(pm => pm.IsSelected == true)).ToList();
                var pmNotificados = MapPmNotificado(selectedItemsSource, SharedData.AutoApproveProduccion,
                                                    SharedData.AutoApproveInventario, FechaActual,
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

        private List<OrdenProduccionMp> MapOrdenProduccionMpSelected(ObservableCollection<OrdenProduccionMaterialGroup> itemsGroupSource, List<OrdenProduccionMaterialGroup> itemsGroup)
        {
            var selectedItems = new List<OrdenProduccionMp>();

            foreach (var itemGroupSource in itemsGroupSource)
            {
                foreach (var uiItem in itemGroupSource.Where(item => item.IsSelected))
                {
                    // Busca el mismo item en la copia "original"
                    var originalItem = itemsGroup
                        .SelectMany(group => group)
                        .FirstOrDefault(item => item.Id == uiItem.Id);

                    if (originalItem != null)
                    {
                        // Calcula la diferencia entre el valor nuevo (UI) y el original
                        var incremento = uiItem.Notificado - originalItem.Notificado;
                        originalItem.Notificado += incremento; // Actualiza solo si es necesario
                        selectedItems.Add(originalItem);
                    }
                }
            }

            return selectedItems;
        }

        private List<MpNotificado> MapPmNotificado(List<OrdenProduccionMp> ordenesProduccionMp, bool AutoApproveProduccion, bool AutoApproveInventario,
                                                   DateTime fecha, string codigoEmpleado, string codigoUsuario)
        {
            return ordenesProduccionMp
                .Select(item => new MpNotificado
                {
                    CodigoMaterial = item.CodigoMaterial,
                    CodigoProduccion = item.CodigoProduccion,
                    Orden = item.Orden,
                    IdMaterialProduccion = item.IdMaterialProduccion,
                    Id = item.Id,                // Mapea el Id
                    Notificado = item.Notificado, // Mapea el Notificado (usa 0 si es null)
                    AprobarAutoProduccion = AutoApproveProduccion,
                    AprobarAutoInventario = AutoApproveInventario,
                    CodigoEmpleado = codigoEmpleado,
                    Fecha = fecha,
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
