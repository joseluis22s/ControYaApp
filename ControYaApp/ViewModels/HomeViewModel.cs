using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using ControYaApp.Models;
using ControYaApp.Services.RestService;

namespace ControYaApp.ViewModels
{
    [QueryProperty(nameof(NombreUsuario), "NombreUsuario")]
    public partial class HomeViewModel : ViewModelBase
    {
        private string? _nombreUsuario;

        private ObservableCollection<PrdOrdenProduccion> _prdOrdenesProduccion;
        public string? NombreUsuario
        {
            get => _nombreUsuario;
            set
            {
                SetProperty(ref _nombreUsuario, value);
            }
        }


        private readonly RestService _restService;


        public ObservableCollection<PrdOrdenProduccion> OrdenesProduccion
        {
            get => _prdOrdenesProduccion;
            set => SetProperty(ref _prdOrdenesProduccion, value);
        }



        public ICommand ObtenerPedidosCommand { get; }

        public HomeViewModel(RestService restService)
        {
            var fecha = DateTime.Now;
            ObtenerPedidosCommand = new RelayCommand(ObtenerPedidosAsync);

            _restService = restService;

        }




        public void ObtenerPedidosAsync()
        {
            OrdenesProduccion = new ObservableCollection<PrdOrdenProduccion>
            {
                new PrdOrdenProduccion
                {
                    CodigoProduccion = "PRDBAL",
                    Orden = 1,
                    Ejercicio = 2024,
                    Periodo = 9,
                    Fecha = DateTime.Now,
                    Referencia = "Balanceado",
                    Detalle = "BALANCEADO ENGORDE - INICIAL",
                    Estado = "A"
                },
                new PrdOrdenProduccion
                {
                    CodigoProduccion = "PRDBAL",
                    Orden = 2,
                    Ejercicio = 2024,
                    Periodo = 9,
                    Fecha = DateTime.Now,
                    Referencia = "Balanceado",
                    Detalle = "BALANCEADO INICIAL - CRECIMIENTO - ENGORDE",
                    Estado = "A"
                },
                new PrdOrdenProduccion
                {
                    CodigoProduccion = "PRDBAL",
                    Orden = 3,
                    Ejercicio = 2024,
                    Periodo = 9,
                    Fecha = DateTime.Now,
                    Referencia = "Balanceado",
                    Detalle = "BALANCEADO ENGORDE",
                    Estado = "A"
                }
            };
            int c = OrdenesProduccion.Count;
        }
    }
}
