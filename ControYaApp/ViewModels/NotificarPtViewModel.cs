using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.Input;
using ControYaApp.Models;
using ControYaApp.Services.LocalDatabase.Repositories;
using ControYaApp.Services.WebService;

namespace ControYaApp.ViewModels
{
    [QueryProperty(nameof(OrdenProduccion), "orden")]
    [QueryProperty(nameof(Empleados), "empleados")]
    public class NotificarPtViewModel : BaseViewModel
    {

        private bool _isNotified { get; set; } = false;



        private readonly PeriodoRepo _periodoRepo;

        private readonly OrdenRepo _ordenRepo;

        private readonly PtNotificadoRepo _ptNotificadoRepo;

        private readonly RestService _restService;



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


        private OrdenProduccion? _ordenProduccion;
        public OrdenProduccion? OrdenProduccion
        {
            get => _ordenProduccion;
            set => SetProperty(ref _ordenProduccion, value);
        }



        public ICommand GoBackCommand { get; }

        public ICommand NotificarPtCommand { get; }




        public NotificarPtViewModel(RestService restService, PtNotificadoRepo ptNotificadoRepo,
                                    OrdenRepo ordenRepo, PeriodoRepo periodoRepo)
        {

            _restService = restService;
            _ptNotificadoRepo = ptNotificadoRepo;
            _ordenRepo = ordenRepo;
            _periodoRepo = periodoRepo;


            GoBackCommand = new AsyncRelayCommand(() => GoBackAsync(_isNotified));
            NotificarPtCommand = new AsyncRelayCommand(NotificarPtAsync);


            InitializeRangoPeriodosAsync();
        }




        private async void InitializeRangoPeriodosAsync()
        {
            RangoPeriodos = await GetRangosPeriodosAsync();
            OrdenProduccion.Fecha = DateTime.Now;
        }


        private async Task GoBackAsync(bool isNotified)
        {
            var navParameter = new ShellNavigationQueryParameters
            {
                { "esNotificado", isNotified}
            };
            await Shell.Current.GoToAsync("..", navParameter);
        }


        private async Task GoBackAsync(bool isNotified, PtNotificadoReq producto)
        {
            var navParameter = new ShellNavigationQueryParameters
            {
                { "esNotificado", isNotified},
                { "productoT", producto}
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

                await _ordenRepo.UpdateOrdenNotificado(notificarProducto);
                await _ptNotificadoRepo.SaveUpdatePtNotificadoAsync(notificarProducto);

                NetworkAccess accessType = Connectivity.Current.NetworkAccess;
                if (accessType == NetworkAccess.Internet)
                {
                    // TODO: Preguntar que criterio deberia no mostrar ordenPt para eliminar los registros de la db.
                    await _restService.NotificarProductoTerminadoAsync(notificarProducto);
                    await _ptNotificadoRepo.SetSincPtNotificadoAsync(notificarProducto);
                }
                // TODO: Mostar ventana con los datos que se van a notificar
                await GoBackAsync(true, notificarProducto);
            }
            catch (Exception ex)
            {
                await Toast.Make(ex.Message).Show();
            }

        }


        private PtNotificadoReq MapPtNotificado(OrdenProduccion? orden, string? empleado, string? serie)
        {
            return new PtNotificadoReq
            {
                CodigoProduccion = orden.CodigoProduccion,
                Orden = orden.Orden,
                CodigoMaterial = orden.CodigoMaterial,
                // TODO: Verificar si al cambiar en la UI, tambien cambio en el viewmodel.
                //       En caso de no ser así, crear una propiedad para el DatePicker
                //       y el Picker, para pasarlo a este método.
                //       Los campos de PtNotificadoque deben verificarse son:
                //       Fecha, Notificado, CodigoEmpleado, Serie y Usuario
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
