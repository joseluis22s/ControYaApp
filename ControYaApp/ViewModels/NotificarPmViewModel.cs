using System.Collections.ObjectModel;
using System.Windows.Input;
using CbMovil.Models;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Input;
using ControYaApp.Models;
using ControYaApp.Services.Dialog;
using ControYaApp.Services.LocalDatabase;
using ControYaApp.Services.Navigation;
using ControYaApp.Services.OrdenProduccionFilter;
using ControYaApp.Services.SharedData;
using ControYaApp.ViewModels.Base;

namespace ControYaApp.ViewModels
{
    [QueryProperty(nameof(OrdenesProduccionMaterialGroup), "ordenesProdMpGrouped")]
    [QueryProperty(nameof(OrdenesProduccionMaterialGroupSource), "ordenesProdMpGroupedSource")]
    [QueryProperty(nameof(RangoPeriodos), "rangosPeriodos")]
    [QueryProperty(nameof(Empleados), "empleados")]
    [QueryProperty(nameof(Lotes), "lotes")]
    public partial class NotificarPmViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly PrdDbReposService _prdDbReposService;

        public ISharedData SharedData { get; set; }


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
                        var saldo = item.Saldo;
                        item.Notificado = item.Cantidad = saldo;
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

        private List<Lote> _lotes;
        public List<Lote> Lotes
        {
            get => _lotes;
            set => SetProperty(ref _lotes, value);
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




        public NotificarPmViewModel(INavigationService navigationService, IDialogService dialogService,
            PrdDbReposService prdDbReposService, ISharedData sharedData, OrdenProduccionMpFilter ordenProduccionMpFilter) : base(navigationService)
        {
            _dialogService = dialogService;
            _prdDbReposService = prdDbReposService;

            SharedData = sharedData;


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

        private async Task NotificarPm()
        {
            try
            {
                // Verifica si hay al menos un ítem seleccionado
                var selectedItems = OrdenesProduccionMaterialGroupSource
                    .SelectMany(g => g.Where(i => i.IsSelected))
                    .ToList();

                if (selectedItems == null || selectedItems.Count == 0)
                {
                    await _dialogService.ShowToastAsync("Ningún item seleccionado.", ToastDuration.Long);
                    return;
                }

                if (EmpleadoSelected is null)
                {
                    await _dialogService.ShowToastAsync("Ningún empleado seleccionado.", ToastDuration.Long);
                    return;
                }

                if (SharedData.EnableLotes)
                {
                    var isLoteSelected = selectedItems.All(i => i.SelectedLote == null);
                    if (isLoteSelected)
                    {
                        await _dialogService.ShowToastAsync("Debe elegir un lote");
                        return;
                    }
                }
                else
                {
                    var items = selectedItems.Where(i => string.IsNullOrWhiteSpace(i.SerieLote)).ToList();
                    if (items.Count != 0)
                    {
                        foreach (var item in items)
                        {
                            item.Detalles = string.Empty;
                            item.SerieLote = string.Empty;
                        };
                    }
                }

                // Actualizamos los valores originales
                ActualizarNotificadosEnOriginales();

                // Obtenemos los items originales seleccionados
                var selectedOriginalItems = OrdenesProduccionMaterialGroup
                    .SelectMany(g => g.Where(i => selectedItems.Any(si => si.Id == i.Id)))
                    .ToList();

                // Mapeamos a PmNotificado
                var pmNotificados = MapPmNotificado(selectedItems,
                                                  SharedData.AutoApproveProduccion,
                                                  SharedData.AutoApproveInventario,
                                                  FechaActual,
                                                  EmpleadoSelected.CodigoEmpleado,
                                                  SharedData.UsuarioSistema);

                int updatedCount = await _prdDbReposService.OrdenProduccionMpRepo.UpdateSelectedNotificadoAsync(selectedOriginalItems);

                int savedOrdUpdateCount = await _prdDbReposService.MpNotificadoRepo.SaveMpNotificadosAsync(pmNotificados);

                await _dialogService.ShowToastAsync($"{updatedCount} items actualizados y {savedOrdUpdateCount} notificados.", ToastDuration.Long);

                await NavigationService.GoBackAsync();

            }
            catch (Exception ex)
            {
                await _dialogService.ShowToastAsync(ex.Message, ToastDuration.Long);
            }
        }

        private void ActualizarNotificadosEnOriginales()
        {
            if (OrdenesProduccionMaterialGroup == null || OrdenesProduccionMaterialGroupSource == null)
                return;

            foreach (OrdenProduccionMaterialGroup sourceGroup in OrdenesProduccionMaterialGroupSource)
            {
                foreach (OrdenProduccionMp sourceItem in sourceGroup.Where(i => i.IsSelected))
                {
                    OrdenesProduccionMaterialGroup
                        .SelectMany(g => g)
                        .FirstOrDefault(i => i.Id == sourceItem.Id).Notificado += sourceItem.Notificado;
                }
            }
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
                    Producto = item.Producto,
                    IdMaterialProduccion = item.IdMaterialProduccion,
                    Notificado = item.Notificado,
                    AprobarAutoProduccion = AutoApproveProduccion,
                    AprobarAutoInventario = AutoApproveInventario,
                    CodigoEmpleado = codigoEmpleado,
                    Fecha = fecha,
                    CodigoUsuario = codigoUsuario,
                    Detalles = item.Detalles,
                    SerieLote = item.SerieLote
                })
                .ToList();
        }

        private async Task GoBackAsync()
        {
            await NavigationService.GoBackAsync();
        }

    }
}
