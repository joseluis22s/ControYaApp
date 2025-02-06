using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.Input;
using ControYaApp.Models;
using ControYaApp.Services.LocalDatabase.Repositories;
using ControYaApp.Services.WebService;
using ControYaApp.Services.WebService.ModelReq;

namespace ControYaApp.ViewModels
{
    [QueryProperty(nameof(OrdenProduccion), "orden")]
    [QueryProperty(nameof(Empleados), "empleados")]
    public class NotificarPtViewModel : ViewModelBase
    {
        private readonly OrdenRepo _ordenRepo;

        private readonly PtNotificadoRepo _ptNotificadoRepo;

        private readonly RestService _restService;


        private string? _serie;

        private string? _empleado;

        private ObservableCollection<string>? _empleados;

        private OrdenProduccion? _ordenProduccion;



        private bool IsNotified { get; set; } = false;


        public string? Serie
        {
            get => _serie;
            set => SetProperty(ref _serie, value);
        }

        public string? Empleado
        {
            get => _empleado;
            set => SetProperty(ref _empleado, value);
        }

        public ObservableCollection<string>? Empleados
        {
            get => _empleados;
            set => SetProperty(ref _empleados, value);
        }

        public OrdenProduccion? OrdenProduccion
        {
            get => _ordenProduccion;
            set => SetProperty(ref _ordenProduccion, value);
        }


        public ICommand GoBackCommand { get; }

        public ICommand NotificarPtCommand { get; }


        public NotificarPtViewModel(RestService restService, PtNotificadoRepo ptNotificadoRepo, OrdenRepo ordenRepo)
        {

            GoBackCommand = new AsyncRelayCommand(() => GoBackAsync(IsNotified));
            NotificarPtCommand = new AsyncRelayCommand(NotificarPtAsync);


            _restService = restService;
            _ptNotificadoRepo = ptNotificadoRepo;
            _ordenRepo = ordenRepo;
        }

        private async Task GoBackAsync(bool isNotified)
        {
            var navParameter = new ShellNavigationQueryParameters
            {
                { "esNotificado", isNotified}
            };
            await Shell.Current.GoToAsync("..", navParameter);
        }

        private async Task GoBackAsync(bool isNotified, PtNotificado producto)
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


                await _ordenRepo.UpdateOrdenNotificado(notificarProducto);
                await _ptNotificadoRepo.SaveUpdatePtNotificadoAsync(notificarProducto);

                NetworkAccess accessType = Connectivity.Current.NetworkAccess;
                if (accessType == NetworkAccess.Internet)
                {
                    // TODO: Preguntar que criterio deberia no mostrar ordenPt para eliminar los registros de la db.
                    await _restService.NotificarPtAsync(notificarProducto);
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


        private PtNotificado MapPtNotificado(OrdenProduccion? orden, string? empleado, string? serie)
        {
            return new PtNotificado
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
    }
}
