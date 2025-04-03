using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Input;
using ControYaApp.Models;
using ControYaApp.Services.LocalDatabase.Repositories;
using ControYaApp.Services.Navigation;
using ControYaApp.Services.Pdf;
using ControYaApp.Services.SharedData;
using ControYaApp.Services.WebService;
using ControYaApp.ViewModels.Base;

namespace ControYaApp.ViewModels
{
    [QueryProperty(nameof(OrdenProduccionPt), "ordenProduccionPt")]
    [QueryProperty(nameof(Empleados), "empleados")]
    public class NotificarPtViewModel : BaseViewModel
    {

        private PdfService _pdfService;

        private readonly PeriodoRepo _periodoRepo;

        private readonly OrdenProduccionPtRepo _ordenProduccionPtRepo;

        private readonly PtNotificadoRepo _ptNotificadoRepo;

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


        private string? _serie;
        public string? Serie
        {
            get => _serie;
            set => SetProperty(ref _serie, value);
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



        public ICommand GoBackCommand { get; }

        public ICommand NotificarPtCommand { get; }

        public ICommand GenerarPdfCommand { get; }



        public NotificarPtViewModel(INavigationService navigationService, RestService restService, PtNotificadoRepo ptNotificadoRepo,
                                    OrdenProduccionPtRepo ordenProduccionPtRepo, PeriodoRepo periodoRepo,
                                    PdfService pdfService, ISharedData sharedData) : base(navigationService)
        {
            SharedData = sharedData;
            _pdfService = pdfService;
            _restService = restService;
            _ptNotificadoRepo = ptNotificadoRepo;
            _ordenProduccionPtRepo = ordenProduccionPtRepo;
            _periodoRepo = periodoRepo;

            GoBackCommand = new AsyncRelayCommand(GoBackAsync);
            NotificarPtCommand = new AsyncRelayCommand(NotificarPtAsync);
            GenerarPdfCommand = new AsyncRelayCommand(GenerarPdf);

            InitializeRangoPeriodosAsync();
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

                var ptNotificado = MapPtNotificado(OrdenProduccionPt, EmpleadoSelected.CodigoEmpleado, Serie, Notificado);
                OrdenProduccionPt.Notificado += ptNotificado.Notificado;
                await _ordenProduccionPtRepo.UpdateNotificadoAsync(OrdenProduccionPt);

                await _ptNotificadoRepo.SavePtNotificadoAsync(ptNotificado);

                _isNotified = true;

                var res = await Shell.Current.DisplayAlert($"Se ha notificado la cantidad {Notificado}", "¿Desea generar un pdf?", "Aceptar", "No generar");
                if (res)
                {
                    //await Toast.Make("Aqui se genera un PDF", ToastDuration.Long).Show();
                    await GenerarPdf();
                    // TODO: Poenr un go back aqui desepes del PDf.
                    //return;
                }
                await GoBackAsync();
                return;

            }
            catch (Exception ex)
            {
                await Toast.Make(ex.Message).Show();
            }

        }

        private async Task<bool> ValidarNotificacionPt()
        {
            if (Saldo < 0)
            {
                await Toast.Make("Saldo agotado").Show();
                return false;
            }

            //if (OrdenProduccionPt.Notificado > OrdenProduccionPt.Saldo)
            //{
            //    await Toast.Make("Valor de notificado mayor al límite").Show();
            //    return false;
            //}
            if (Convert.ToDecimal(Notificado) > Cantidad)
            {
                await Toast.Make("Valor a notificar excede el límite").Show();
                return false;
            }

            if (Convert.ToDecimal(Notificado) <= 0)
            {
                await Toast.Make("Valor a notificar no válido").Show();
                return false;
            }

            if (EmpleadoSelected is null)
            {
                await Toast.Make($"Debe elegir un empleado").Show();
                return false;
            }

            if (string.IsNullOrWhiteSpace(Serie))
            {
                Serie = "";
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
                await Toast.Make($"Botón para generar un pdf", ToastDuration.Long).Show();
            }
            catch (Exception ex)
            {
                await Toast.Make(ex.Message).Show();
            }
        }

        private PtNotificado MapPtNotificado(OrdenProduccionPt ordenProduccionPt, string? codigoEmpleado, string? serie, decimal notificado)
        {
            return new PtNotificado
            {
                CodigoProduccion = ordenProduccionPt.CodigoProduccion,
                Orden = ordenProduccionPt.Orden,
                CodigoMaterial = ordenProduccionPt.CodigoMaterial,
                Fecha = FechaActual,
                Producto = ordenProduccionPt.Producto,
                // TODO: Eliminar esat linea
                // Notificado = ordenProduccionPt.Notificado + notificado,
                Notificado = notificado,
                CodigoEmpleado = codigoEmpleado,
                Serie = serie,
                CodigoUsuario = ordenProduccionPt.CodigoUsuarioAprobar,
                AprobarAutoProduccion = SharedData.AutoApproveProduccion,
                AprobarAutoInventario = SharedData.AutoApproveInventario
            };
        }

        private async Task<Periodos> GetRangosPeriodosAsync()
        {
            return await _periodoRepo.GetRangosPeriodosAsync();
        }




    }
}
