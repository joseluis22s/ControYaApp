using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.Input;
using ControYaApp.Models;
using ControYaApp.Services.LocalDatabase.Repositories;
using ControYaApp.Services.Pdf;
using ControYaApp.Services.WebService;

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




        private bool _isNotified;



        private decimal? _cantidad;
        public decimal? Cantidad
        {
            get => _cantidad;
            set => SetProperty(ref _cantidad, value);
        }


        private decimal? _notificado;
        public decimal? Notificado
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


        public decimal? Saldo => Cantidad - Notificado;


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
                Cantidad = _ordenProduccionPt?.Saldo;
                Notificado = _ordenProduccionPt?.Saldo;
            }
        }



        public ICommand GoBackCommand { get; }

        public ICommand NotificarPtCommand { get; }

        public ICommand GenerarPdfCommand { get; }



        public NotificarPtViewModel(RestService restService, PtNotificadoRepo ptNotificadoRepo,
                                    OrdenProduccionPtRepo ordenProduccionPtRepo, PeriodoRepo periodoRepo, PdfService pdfService)
        {

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
            await Shell.Current.GoToAsync("..", navParameter);
        }

        private async Task GoBackNotifiedFalseAsync(bool isNotified)
        {
            var navParameter = new ShellNavigationQueryParameters
            {
                { "esNotificado", isNotified}
            };
            await Shell.Current.GoToAsync("..", navParameter);
        }

        private async Task NotificarPtAsync()
        {
            try
            {
                if (!await ValidarNotificacionPt())
                {
                    return;
                }

                var ptNotificado = MapPtNotificado(OrdenProduccionPt, EmpleadoSelected.CodigoEmpleado, Serie);

                await _ordenProduccionPtRepo.UpdateNotificadoAsync(OrdenProduccionPt);

                await _ptNotificadoRepo.SaveOrUpdatePtNotificadoAsync(ptNotificado);

                _isNotified = true;

                var res = await Shell.Current.DisplayAlert($"Se ha notificado la cantidad {Notificado}", "¿Desea generar un pdf?", "Aceptar", "Salir");
                if (res)
                {
                    await GenerarPdf();
                    return;
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
            if (OrdenProduccionPt.Saldo == 0)
            {
                await Toast.Make("No se puede notificar más").Show();
                return false;
            }

            //if (OrdenProduccionPt.Notificado > OrdenProduccionPt.Saldo)
            //{
            //    await Toast.Make("Valor de notificado mayor al límite").Show();
            //    return false;
            //}

            if (OrdenProduccionPt.Notificado <= 0)
            {
                await Toast.Make("Valor de notificado no válido").Show();
                return false;
            }

            if (string.IsNullOrEmpty(EmpleadoSelected.NombreEmpleado))
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
                await Toast.Make($"Botón para generar un pdf").Show();
            }
            catch (Exception ex)
            {
                await Toast.Make(ex.Message).Show();
            }
        }

        private PtNotificado MapPtNotificado(OrdenProduccionPt ordenProduccionPt, string? codigoEmpleado, string? serie)
        {
            return new PtNotificado
            {
                CodigoProduccion = ordenProduccionPt.CodigoProduccion,
                Orden = ordenProduccionPt.Orden,
                CodigoMaterial = ordenProduccionPt.CodigoMaterial,
                Fecha = FechaActual,
                Notificado = ordenProduccionPt.Notificado,
                CodigoEmpleado = codigoEmpleado,
                Serie = serie,
                Usuario = ordenProduccionPt.CodigoUsuarioAprobar
            };
        }

        private async Task<Periodos> GetRangosPeriodosAsync()
        {
            return await _periodoRepo.GetRangosPeriodosAsync();
        }




    }
}
