using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ControYaApp.Models;
using ControYaApp.Services.LocalDatabase.Repositories;
using ControYaApp.Services.WebService;
using ControYaApp.Views.Controls;

namespace ControYaApp.ViewModels
{
    [QueryProperty(nameof(Usuario), "usuario")]
    [QueryProperty(nameof(EsNotificado), "esNotificado")]
    [QueryProperty(nameof(Productot), "productoT")]
    public partial class OrdenesViewModel : ViewModelBase
    {
        private readonly OrdenRepo _ordenRepo;

        private readonly EmpleadosRepo _empleadosRepo;



        private Usuario? _usuario;

        private ObservableCollection<OrdenProduccionCabecera>? _ordenesProduccion;



        public bool EsNotificado { get; set; }
        public PtNotificadoReq Productot { get; set; }



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

        //public ICommand AppearingCommand { get; }


        public OrdenesViewModel(RestService restService, OrdenRepo ordenRepo, EmpleadosRepo empleadosRepo)
        {
            ObtenerPedidosCommand = new AsyncRelayCommand(ObtenerPedidosAsync);
            NotificarPtCommand = new AsyncRelayCommand<OrdenProduccionDetalle>(NotificarPtAsync);

            IsActive = true;

            WeakReferenceMessenger.Default.Register<ClearDataMessage>(this, (r, m) =>
            {
                if (m.Value == "Vaciar")
                {
                    OrdenesProduccion?.Clear();
                }
            });

            _ordenRepo = ordenRepo;
            _empleadosRepo = empleadosRepo;
        }

        internal void Appearing()
        {
            try
            {
                if (EsNotificado)
                {
                    OrdenesProduccion.Where(o =>
                        Productot.CodigoProduccion == o.CodigoProduccion &&
                        Productot.Orden == o.Orden
                        ).FirstOrDefault().Detalles.Where(d =>
                                Productot.CodigoMaterial == d.CodigoMaterial).FirstOrDefault().Notificado += Productot.Notificado;
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }
        }

        public async Task ObtenerPedidosAsync()
        {
            var loadingPopUpp = new LoadingPopUp();
            _ = Shell.Current.CurrentPage.ShowPopupAsync(loadingPopUpp);

            try
            {
                var ordenesDb = await _ordenRepo.GetOrdenesByUsuarioSistema(Usuario.UsuarioSistema);

                if (ordenesDb.Count != 0)
                {
                    OrdenesProduccion = MapOrdenesCabeceras(ordenesDb);
                }
                else
                {
                    await Toast.Make("No se han extraido datos").Show();
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
            try
            {
                var empleados = await _empleadosRepo.GetAllEmpleadosAsync();

                empleados = empleados.Order().ToObservableCollection();

                var orden = MapOrdenProduccion(detalle);
                var navParameter = new ShellNavigationQueryParameters
            {
                { "orden", orden},
                { "empleados", empleados}
            };
                await Shell.Current.GoToAsync("notificarPt", navParameter);
            }
            catch (Exception ex)
            {
                await Toast.Make(ex.Message).Show();
            }
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
