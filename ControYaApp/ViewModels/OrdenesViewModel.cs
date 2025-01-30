using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using ControYaApp.Models;
using ControYaApp.Services.LocalDatabase.Repositories;
using ControYaApp.Services.WebService;
using ControYaApp.Views.Controls;

namespace ControYaApp.ViewModels
{
    [QueryProperty(nameof(NombreUsuario), "NombreUsuario")]
    public partial class OrdenesViewModel : ViewModelBase
    {
        private OrdenRepo _ordenRepo;

        private readonly RestService _restService;



        private string? _nombreUsuario;

        private ObservableCollection<OrdenProduccionCabecera>? _ordenesProduccion;



        public string? NombreUsuario
        {
            get => _nombreUsuario;
            set
            {
                SetProperty(ref _nombreUsuario, value);
            }
        }

        public ObservableCollection<OrdenProduccionCabecera>? OrdenesProduccion
        {
            get => _ordenesProduccion;
            set => SetProperty(ref _ordenesProduccion, value);
        }

        public OrdenProduccionDetalle? SelectedOrdenDetalle { get; set; }



        public ICommand ObtenerPedidosCommand { get; }

        public ICommand NotificarPtCommand { get; }


        public OrdenesViewModel(RestService restService, OrdenRepo ordenRepo)
        {
            var fecha = DateTime.Now;
            ObtenerPedidosCommand = new AsyncRelayCommand(ObtenerPedidosAsync);
            NotificarPtCommand = new AsyncRelayCommand<OrdenProduccionDetalle>(NotificarPtAsync);

            _restService = restService;
            _ordenRepo = ordenRepo;
        }


        public async Task ObtenerPedidosAsync()
        {
            var loadingPopUpp = new LoadingPopUp();
            _ = Shell.Current.CurrentPage.ShowPopupAsync(loadingPopUpp);

            NetworkAccess accessType = Connectivity.Current.NetworkAccess;

            if (accessType == NetworkAccess.Internet)
            {
                ObservableCollection<OrdenProduccion> listaOrdenes = await _restService.GetAllOrdenesProduccionAsync(NombreUsuario);

                if (listaOrdenes.Count == 0)
                {
                    await Toast.Make("No se han cargado órdenes").Show();
                    return;
                }
                else
                {
                    // TODO: Verificar si se ncesita limpiar la base antes de "sincronizar"
                    OrdenesProduccion = MapOrdenesCabeceras(listaOrdenes);
                    await _ordenRepo.SaveOrdenesAsync(listaOrdenes);
                }
            }
            else
            {
                await Toast.Make("Trabajando sin red").Show();

                var listaOrdenes = await _ordenRepo.GetOrdenesByUsuario(NombreUsuario);
                if (listaOrdenes.Count == 0)
                {
                    await Toast.Make("No se han cargado órdenes").Show();
                }
                else
                {
                    OrdenesProduccion = MapOrdenesCabeceras(listaOrdenes);
                }
            }

            await loadingPopUpp.CloseAsync();

        }

        private ObservableCollection<OrdenProduccionCabecera> MapOrdenesCabeceras(ObservableCollection<OrdenProduccion> ordenesProduccion)
        {
            // Agrupa las órdenes por las propiedades de cabecera
            var agrupaciones = ordenesProduccion
                .GroupBy(op => new
                {
                    op.Centro,
                    op.CodigoProduccion,
                    op.Orden,
                    op.Fecha,
                    op.Referencia,
                });

            // Mapea cada grupo a una cabecera
            var cabeceras = agrupaciones.Select(grupo => new OrdenProduccionCabecera
            {
                Centro = grupo.Key.Centro,
                CodigoProduccion = grupo.Key.CodigoProduccion,
                Orden = grupo.Key.Orden,
                Fecha = grupo.Key.Fecha,
                Referencia = grupo.Key.Referencia,
                Detalles = new ObservableCollection<OrdenProduccionDetalle>(
                    grupo.Select(op => new OrdenProduccionDetalle
                    {
                        Detalle = op.Detalle,
                        CodigoMaterial = op.CodigoMaterial,
                        CodigoProducto = op.CodigoProducto,
                        Producto = op.Producto,
                        CodigoUnidad = op.CodigoUnidad,
                        Cantidad = op.Cantidad,
                        Notificado = op.Notificado,
                    }))
            });

            // Convierte la lista a ObservableCollection
            return new ObservableCollection<OrdenProduccionCabecera>(cabeceras);
        }

        public async Task NotificarPtAsync(OrdenProduccionDetalle? detalles)
        {

            var navParameter = new ShellNavigationQueryParameters
            {
                { "detalles", detalles }
            };
            await Shell.Current.GoToAsync("notificarPt", navParameter);
        }
    }
}
