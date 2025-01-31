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
    [QueryProperty(nameof(Usuario), "usuario")]
    public partial class OrdenesViewModel : ViewModelBase
    {
        private readonly OrdenRepo _ordenRepo;

        private readonly RestService _restService;



        private Usuario? _usuario;

        private ObservableCollection<OrdenProduccionCabecera>? _ordenesProduccion;



        public Usuario? Usuario
        {
            get => _usuario;
            set
            {
                SetProperty(ref _usuario, value);
            }
        }

        public ObservableCollection<OrdenProduccionCabecera>? OrdenesProduccion
        {
            get => _ordenesProduccion;
            set => SetProperty(ref _ordenesProduccion, value);
        }




        public ICommand ObtenerPedidosCommand { get; }

        public ICommand NotificarPtCommand { get; }


        public OrdenesViewModel(RestService restService, OrdenRepo ordenRepo)
        {
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

            try
            {
                if (accessType == NetworkAccess.Internet)
                {
                    ObservableCollection<OrdenProduccion> listaOrdenes = await _restService.GetAllOrdenesProduccionByUsuarioAsync(Usuario.UsuarioSistema);

                    if (listaOrdenes.Count == 0)
                    {
                        await Toast.Make("No se han cargado órdenes").Show();
                    }
                    else
                    {
                        OrdenesProduccion = MapOrdenesCabeceras(listaOrdenes);
                        await _ordenRepo.SaveOrdenesAsync(listaOrdenes);
                    }
                }
                else
                {
                    await Toast.Make("Trabajando sin conexión").Show();

                    var listaOrdenes = await _ordenRepo.GetOrdenesByUsuario(Usuario.UsuarioSistema);
                    if (listaOrdenes.Count == 0)
                    {
                        await Toast.Make("No se han cargado órdenes").Show();
                    }
                    else
                    {
                        OrdenesProduccion = MapOrdenesCabeceras(listaOrdenes);
                    }
                }
            }
            catch (Exception ex)
            {
                await Toast.Make(ex.Message).Show();
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

            var cabeceras = agrupaciones.Select(grupo =>
            {
                var cabecera = new OrdenProduccionCabecera
                {
                    Centro = grupo.Key.Centro,
                    CodigoProduccion = grupo.Key.CodigoProduccion,
                    Orden = grupo.Key.Orden,
                    Fecha = grupo.Key.Fecha,
                    Referencia = grupo.Key.Referencia,
                    Detalles = new ObservableCollection<OrdenProduccionDetalle>()
                };

                foreach (var op in grupo)
                {
                    var detalle = new OrdenProduccionDetalle
                    {
                        Detalle = op.Detalle,
                        CodigoMaterial = op.CodigoMaterial,
                        CodigoProducto = op.CodigoProducto,
                        Producto = op.Producto,
                        CodigoUnidad = op.CodigoUnidad,
                        Cantidad = op.Cantidad,
                        Notificado = op.Notificado,
                        Cabecera = cabecera // Asignar la cabecera aquí
                    };
                    cabecera.Detalles.Add(detalle);
                }

                return cabecera;
            });

            return new ObservableCollection<OrdenProduccionCabecera>(cabeceras);
        }

        public async Task NotificarPtAsync(OrdenProduccionDetalle? detalle)
        {
            var orden = MapOrdenProduccion(detalle);
            var navParameter = new ShellNavigationQueryParameters
            {
                { "orden", orden}
            };
            await Shell.Current.GoToAsync("notificarPt", navParameter);
        }

        private OrdenProduccion MapOrdenProduccion(OrdenProduccionDetalle detalle)
        {
            return new OrdenProduccion
            {
                Centro = detalle.Cabecera.Centro,
                CodigoProduccion = detalle.Cabecera.CodigoProduccion,
                Orden = detalle.Cabecera.Orden,
                CodigoUsuario = Usuario.UsuarioSistema,
                Fecha = detalle.Cabecera.Fecha,
                Referencia = detalle.Cabecera.Referencia,
                Detalle = detalle.Detalle,
                CodigoMaterial = detalle.CodigoMaterial,
                CodigoProducto = detalle.CodigoProducto,
                Producto = detalle.Producto,
                CodigoUnidad = detalle.CodigoUnidad,
                Cantidad = detalle.Cantidad,
                Notificado = detalle.Saldo
            };
        }
    }
}
