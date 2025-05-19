using System.Collections.ObjectModel;
using System.Windows.Input;
using CbMovil.Models;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Input;
using ControYaApp.Models;
using ControYaApp.Services.AppLocalDatabase;
using ControYaApp.Services.Dialog;
using ControYaApp.Services.LocalDatabase;
using ControYaApp.Services.Navigation;
using ControYaApp.Services.Pdf;
using ControYaApp.Services.SharedData;
using ControYaApp.Services.WebService;
using ControYaApp.ViewModels.Base;

namespace ControYaApp.ViewModels
{
    [QueryProperty(nameof(OrdenProduccionPt), "ordenProduccionPt")]
    [QueryProperty(nameof(Empleados), "empleados")]
    [QueryProperty(nameof(Lotes), "lotes")]
    public partial class NotificarPtViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly AppDbReposService _appDbReposService;
        private readonly PrdDbReposService _prdDbReposService;



        private PdfService _pdfService;


        private readonly RestService _restService;


        public ISharedData SharedData { get; set; }




        private bool _isNotified;



        private decimal _cantidad;
        public decimal Cantidad
        {
            get => _cantidad;
            set => SetProperty(ref _cantidad, value);
        }


        private decimal _notificado;
        public decimal Notificado
        {
            get => _notificado;
            set
            {
                if (SetProperty(ref _notificado, value))
                {
                    OnPropertyChanged(nameof(Saldo));
                }
            }
        }

        private decimal? _saldo;
        public decimal? Saldo
        {
            get => _saldo;
            set
            {
                SetProperty(ref _saldo, value);
            }
        }


        private DateTime _fechaActual = DateTime.Now;
        public DateTime FechaActual
        {
            get => _fechaActual;
            set => SetProperty(ref _fechaActual, value);
        }


        private Periodos _rangoPeriodos;
        public Periodos RangoPeriodos
        {
            get => _rangoPeriodos;
            set => SetProperty(ref _rangoPeriodos, value);
        }


        private Lote? _selectedLote;
        public Lote? SelectedLote
        {
            get => _selectedLote;
            set
            {
                if (SetProperty(ref _selectedLote, value))
                {
                    SerieLote = value?.Nombre;
                }
            }
        }

        private string _serieLote;
        public string SerieLote
        {
            get => _serieLote;
            set => SetProperty(ref _serieLote, value);
        }

        private string? _detalles;
        public string? Detalles
        {
            get => _detalles;
            set => SetProperty(ref _detalles, value);
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


        private OrdenProduccionPt? _ordenProduccionPt;
        public OrdenProduccionPt? OrdenProduccionPt
        {
            get => _ordenProduccionPt;
            set
            {
                SetProperty(ref _ordenProduccionPt, value);
                Saldo = Notificado = Cantidad = _ordenProduccionPt.Saldo;
            }
        }


        private List<Lote> _lotes;
        public List<Lote> Lotes
        {
            get => _lotes;
            set => SetProperty(ref _lotes, value);
        }


        public ICommand GoBackCommand { get; }

        public ICommand NotificarPtCommand { get; }

        public ICommand GenerarPdfCommand { get; }
        public ICommand AddLoteCommand { get; }



        public NotificarPtViewModel(INavigationService navigationService, IDialogService dialogService, RestService restService,
                                    AppDbReposService appDbReposService, PrdDbReposService prdDbReposService,
                                    PdfService pdfService, ISharedData sharedData) : base(navigationService)
        {
            _dialogService = dialogService;
            _appDbReposService = appDbReposService;
            _prdDbReposService = prdDbReposService;


            SharedData = sharedData;
            _pdfService = pdfService;
            _restService = restService;


            GoBackCommand = new AsyncRelayCommand(GoBackAsync);
            NotificarPtCommand = new AsyncRelayCommand(NotificarPtAsync);
            GenerarPdfCommand = new AsyncRelayCommand(GenerarPdf);
            AddLoteCommand = new AsyncRelayCommand(AddLoteAsync);

            InitializeRangoPeriodosAsync();
        }

        private async Task AddLoteAsync()
        {
            try
            {
                var lote = await _dialogService.DisplayPromptAsync("Escriba el nuevo lote", "nombre del lote", "Aceptar", "Cancelar", "Lote");

                if (lote == null)
                    return;

                if (await _prdDbReposService.LoteRepo.FindByNombre(lote))
                {
                    await _dialogService.ShowToastAsync("Nombre de LOTE ya existe");
                    return;
                }

                Lote newLote = new Lote
                {
                    Nombre = lote,
                    Sincronizar = true
                };
                await _prdDbReposService.LoteRepo.SaveLoteAsync(newLote);
                var lotesOrdened = await _prdDbReposService.LoteRepo.GetAllLotesAsync();
                Lotes = lotesOrdened.OrderBy(o => o.Nombre).ToList();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowToastAsync(ex.Message, ToastDuration.Long);
            }
        }


        private async void InitializeRangoPeriodosAsync()
        {
            RangoPeriodos = await GetRangosPeriodosAsync();
        }

        private async Task GoBackAsync()
        {
            await (_isNotified ? GoBackNotifiedTrueAsync(true, OrdenProduccionPt) : GoBackNotifiedFalseAsync(false));
        }

        private async Task GoBackNotifiedTrueAsync(bool isNotified, OrdenProduccionPt ordenProduccionPt)
        {
            var navParameter = new ShellNavigationQueryParameters
            {
                { "esNotificado", isNotified},
                { "ordenProduccionPt", ordenProduccionPt}
            };
            await NavigationService.GoBackAsync(navParameter);
        }

        private async Task GoBackNotifiedFalseAsync(bool isNotified)
        {
            var navParameter = new ShellNavigationQueryParameters
            {
                { "esNotificado", isNotified}
            };
            await NavigationService.GoBackAsync(navParameter);
        }



        private async Task NotificarPtAsync()
        {
            try
            {
                if (!await ValidarNotificacionPt())
                {
                    return;
                }

                var ptNotificado = MapPtNotificado(OrdenProduccionPt, EmpleadoSelected.CodigoEmpleado, SerieLote, Notificado, Detalles);
                OrdenProduccionPt.Notificado += ptNotificado.Notificado;
                await _prdDbReposService.OrdenProduccionPtRepo.UpdateNotificadoAsync(OrdenProduccionPt);

                await _prdDbReposService.PtNotificadoRepo.SavePtNotificadoAsync(ptNotificado);

                _isNotified = true;

                var res = await _dialogService.DisplayAlertAsync($"Se ha notificado la cantidad {Notificado}", "¿Desea generar un pdf?", "Aceptar", "No generar");
                if (res)
                {
                    //await Toast.Make("Aqui se genera un PDF", ToastDuration.Long).Show();
                    await GenerarPdf();
                    // TODO: Poenr un go back aqui desepes del PDf.
                    //return;
                }
                await GoBackAsync();

            }
            catch (Exception ex)
            {
                await _dialogService.ShowToastAsync(ex.Message, ToastDuration.Long);
            }

        }

        private async Task<bool> ValidarNotificacionPt()
        {
            if (Saldo < 0)
            {
                await _dialogService.ShowToastAsync("Saldo agotado");
                return false;
            }

            if (Convert.ToDecimal(Notificado) > Cantidad)
            {
                await _dialogService.ShowToastAsync("Valor a notificar excede el límite");
                return false;
            }

            if (Convert.ToDecimal(Notificado) <= 0)
            {
                await _dialogService.ShowToastAsync("Valor a notificar no válido");
                return false;
            }

            if (EmpleadoSelected is null)
            {
                await _dialogService.ShowToastAsync("Debe elegir un empleado");
                return false;
            }

            if (SharedData.EnableLotes)
            {
                if (SelectedLote is null)
                {
                    await _dialogService.ShowToastAsync("Debe elegir un lote");
                    return false;
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(SerieLote))
                {
                    SerieLote = string.Empty;
                }
            }

            return true;
        }

        // TODO: No modificar si el pdf se elimina con DisplayAlert.
        //       Si es con boton, habilitar el boton.
        private async Task GenerarPdf()
        {
            try
            {
                // TODO: Controlar que notifique la menos una vez para poder generar
                await _dialogService.ShowToastAsync("Acción no implementada", ToastDuration.Long);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowToastAsync(ex.Message, ToastDuration.Long);
            }
        }

        private PtNotificado MapPtNotificado(OrdenProduccionPt ordenProduccionPt, string? codigoEmpleado, string? serieLote, decimal notificado, string detalles)
        {
            return new PtNotificado
            {
                CodigoProduccion = ordenProduccionPt.CodigoProduccion,
                Orden = ordenProduccionPt.Orden,
                CodigoMaterial = ordenProduccionPt.CodigoMaterial,
                Fecha = FechaActual,
                Producto = ordenProduccionPt.Producto,
                Notificado = notificado,
                CodigoEmpleado = codigoEmpleado,
                SerieLote = serieLote,
                CodigoUsuario = ordenProduccionPt.CodigoUsuarioAprobar,
                AprobarAutoProduccion = SharedData.AutoApproveProduccion,
                AprobarAutoInventario = SharedData.AutoApproveInventario,
                Detalles = detalles
            };
        }

        private async Task<Periodos> GetRangosPeriodosAsync()
        {
            return await _appDbReposService.PeriodoRepo.GetRangosPeriodosAsync();
        }




    }
}
