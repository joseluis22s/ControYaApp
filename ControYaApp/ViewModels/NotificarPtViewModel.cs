using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
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

        private readonly OrdenProduccionRepo _ordenRepo;

        private readonly PtNotificadoRepo _ptNotificadoRepo;

        private readonly RestService _restService;



        private bool _isNotified;


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


        private string? _empleado;
        public string? Empleado
        {
            get => _empleado;
            set => SetProperty(ref _empleado, value);
        }


        private ObservableCollection<string>? _empleados;
        public ObservableCollection<string>? Empleados
        {
            get => _empleados;
            set => SetProperty(ref _empleados, value);
        }


        private OrdenProduccionPt? _ordenProduccionPt;
        public OrdenProduccionPt? OrdenProduccionPt
        {
            get => _ordenProduccionPt;
            set => SetProperty(ref _ordenProduccionPt, value);
        }



        public ICommand GoBackCommand { get; }

        public ICommand NotificarPtCommand { get; }

        public ICommand GenerarPdfCommand { get; }



        public NotificarPtViewModel(RestService restService, PtNotificadoRepo ptNotificadoRepo,
                                    OrdenProduccionRepo ordenRepo, PeriodoRepo periodoRepo, PdfService pdfService)
        {

            _pdfService = pdfService;
            _restService = restService;
            _ptNotificadoRepo = ptNotificadoRepo;
            _ordenRepo = ordenRepo;
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
            await (_isNotified ? GoBackNotifiedTrueAsync(true, OrdenProduccion) : GoBackNotifiedFalseAsync(false));
        }

        private async Task GoBackNotifiedFalseAsync(bool isNotified)
        {
            var navParameter = new ShellNavigationQueryParameters
            {
                { "esNotificado", isNotified}
            };
            await Shell.Current.GoToAsync("..", navParameter);
        }


        private async Task GoBackNotifiedTrueAsync(bool isNotified, OrdenProduccion ordenProduccion)
        {
            var navParameter = new ShellNavigationQueryParameters
            {
                { "esNotificado", isNotified},
                { "productoT", ordenProduccion}
            };
            await Shell.Current.GoToAsync("..", navParameter);
        }


        private async Task NotificarPtAsync()
        {
            try
            {
                var notificarProducto = MapPtNotificado(OrdenProduccion, Empleado, Serie);

                if (notificarProducto.Notificado <= 0)
                {
                    await Toast.Make($"Valor de notificado: \'{notificarProducto.Notificado}\' no válido").Show();
                    return;
                }
                else if (string.IsNullOrEmpty(notificarProducto.CodigoEmpleado))
                {
                    await Toast.Make($"Debe elegir un empleado").Show();
                    return;
                }
                else if (string.IsNullOrWhiteSpace(notificarProducto.Serie))
                {
                    notificarProducto.Serie = "";
                }



                NetworkAccess accessType = Connectivity.Current.NetworkAccess;
                if (accessType == NetworkAccess.Internet)
                {
                    // TODO: Preguntar que criterio deberia no mostrar ordenPt para eliminar los registros de la db.
                    var res = await _restService.NotificarProductoTerminadoAsync(notificarProducto);
                    if (res)
                    {
                        // Si hay conexión a internet, se actualiza el campo 'Sincronizado' a 'False' para definir que tambien se hizo en CB.
                        await _ptNotificadoRepo.SynchronizedTruePtNotificadoAsync(notificarProducto);

                        _isNotified = true;

                        await Toast.Make("Producto notificado").Show();
                    }
                    else
                    {
                        _isNotified = false;

                        await Toast.Make("Problemas al notificar el producto").Show();
                    }
                }
                else
                {
                    // Si no hay conexión a internet, se inserta 'Notificado' en la db local.
                    await _ordenRepo.NotificarProductoTerminadoAsync(notificarProducto);
                    // Si no hay conexion a internet, se inserta el registro en la db local.
                    //await _ptNotificadoRepo.SynchronizedFalsePtNotificadoAsync(notificarProducto);
                }


                // TODO: Mostar ventana con los datos que se van a notificar
                //await GoBackAsync(true, notificarProducto);
            }
            catch (Exception ex)
            {
                await Toast.Make(ex.Message).Show();
            }

        }

        private async Task GenerarPdf()
        {
            //try
            //{
            //    var view = Shell.Current.CurrentPage as ContentPage;
            //    var path = _pdfService.GeneratePdf(view.Content, OrdenProduccion.CodigoProduccion, OrdenProduccion.Orden, OrdenProduccion.CodigoMaterial);
            //    if (string.IsNullOrEmpty(path))
            //    {
            //        await Toast.Make("No se genero PDF").Show();
            //    }
            //    else
            //    {
            //        //WebViewPdfSource = path;
            //        await Toast.Make($"PDF generado en: {path}").Show();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    await Toast.Make(ex.Message).Show();
            //}
        }

        private async Task GenerarPdfssssss()
        {
            try
            {
                if (_isNotified)
                {

                    //await Toast.Make("aqui se busca generar el PDF", ToastDuration.Long).Show();
                    //var loadingPopUpp = new LoadingPopUp();
                    //_ = Shell.Current.CurrentPage.ShowPopupAsync(loadingPopUpp);

                    var navParameter = new ShellNavigationQueryParameters
                    {
                        { "orden", OrdenProduccion},
                        { "serie", Serie},
                        { "empleado", Empleado}
                    };
                    await Shell.Current.GoToAsync("notificarPtPdf", navParameter);


                    //await loadingPopUpp.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                await Toast.Make("Error al generar el PDF:\n" + msg, ToastDuration.Long).Show();
            }
        }


        private PtNotificadoReq MapPtNotificado(OrdenProduccion? orden, string? empleado, string? serie)
        {
            return new PtNotificadoReq
            {
                CodigoProduccion = orden.CodigoProduccion,
                Orden = orden.Orden,
                CodigoMaterial = orden.CodigoMaterial,
                Fecha = orden.Fecha,
                Notificado = orden.Notificado,
                CodigoEmpleado = empleado,
                Serie = serie,
                Usuario = orden.CodigoUsuario
            };
        }


        private async Task<Periodos> GetRangosPeriodosAsync()
        {
            return await _periodoRepo.GetRangosPeriodosAsync();
        }




    }
}
