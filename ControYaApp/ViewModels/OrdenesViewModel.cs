using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ControYaApp.Models;
using ControYaApp.Services.LocalDatabase.Repositories;
using ControYaApp.Services.SharedData;
using ControYaApp.Services.WebService;
using ControYaApp.Views.Controls;

namespace ControYaApp.ViewModels
{
    [QueryProperty(nameof(EsNotificado), "esNotificado")]
    [QueryProperty(nameof(OrdenProduccionPt), "productoT")]
    public partial class OrdenesViewModel : BaseViewModel
    {

        public ISharedData SharedData { get; set; }


        public OrdenProduccionPt OrdenProduccionPt { get; set; }

        public bool EsNotificado { get; set; }




        private readonly OrdenProduccionRepo _ordenProduccionRepo;

        private readonly OrdenProduccionPtRepo _ordenProduccionPtRepo;

        private readonly EmpleadosRepo _empleadosRepo;



        private ObservableCollection<OrdenProduccionCabecera>? _ordenesProduccion;

        public ObservableCollection<OrdenProduccionCabecera>? OrdenesProduccion
        {
            get => _ordenesProduccion;
            set => SetProperty(ref _ordenesProduccion, value);
        }

        private ObservableCollection<OrdenProduccionGroup> _ordenesGrouped;
        public ObservableCollection<OrdenProduccionGroup> OrdenesProduccionGrouped
        {
            get => _ordenesGrouped;
            set => SetProperty(ref _ordenesGrouped, value);
        }



        public ICommand ObtenerOrdenesCommand { get; }

        public ICommand NotificarPtCommand { get; }




        public OrdenesViewModel(RestService restService, OrdenProduccionRepo ordenProduccionRepo, OrdenProduccionPtRepo ordenProduccionPtRepo, EmpleadosRepo empleadosRepo, ISharedData sharedData)
        {

            SharedData = sharedData;


            _ordenProduccionRepo = ordenProduccionRepo;
            _ordenProduccionPtRepo = ordenProduccionPtRepo;
            _empleadosRepo = empleadosRepo;


            ObtenerOrdenesCommand = new AsyncRelayCommand(ObtenerOrdenesAsync);
            NotificarPtCommand = new AsyncRelayCommand<OrdenProduccionPt>(NotificarPtAsync);


            VaciarOrdenes();
        }




        private void VaciarOrdenes()
        {
            WeakReferenceMessenger.Default.Register<ClearDataMessage>(this, (r, m) =>
            {
                if (m.Value == "Vaciar")
                {
                    OrdenesProduccion?.Clear();
                }
            });
        }


        internal async void Appearing()
        {
            try
            {
                if (EsNotificado)
                {
                    OrdenesProduccion.FirstOrDefault(o =>
                        OrdenProduccionPt.Centro == o.Centro &&
                        OrdenProduccionPt.CodigoProduccion == o.CodigoProduccion &&
                        OrdenProduccionPt.Orden == o.Orden)
                        .Detalles.FirstOrDefault(d =>
                                OrdenProduccionPt.CodigoProducto == d.CodigoProducto).Notificado += OrdenProduccionPt.Notificado;

                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task ObtenerOrdenesAsync()
        {
            var loadingPopUpp = new LoadingPopUp();
            _ = Shell.Current.CurrentPage.ShowPopupAsync(loadingPopUpp);

            try
            {
                if (OrdenesProduccionGrouped.Count != 0)
                {

                }
                var ordenesProduccionDb = await _ordenProduccionRepo.GetOrdenesProduccionByUsuarioSistema(SharedData.UsuarioSistema);

                if (ordenesProduccionDb.Count != 0)
                {
                    var ordenesProduccionPt = await _ordenProduccionPtRepo.GetAllOrdenesProduccionPt();
                    OrdenesProduccionGrouped = MapOrdenesProduccionGrouped(ordenesProduccionDb, ordenesProduccionPt);
                }
                else
                {
                    await Toast.Make("No se han encontrado datos").Show();
                }
            }
            catch (Exception ex)
            {
                await Toast.Make(ex.Message).Show();
            }
            finally
            {
                await loadingPopUpp.CloseAsync();
            }

        }


        public async Task NotificarPtAsync(OrdenProduccionPt ordenProduccionPt)
        {
            try
            {
                var empleados = await _empleadosRepo.GetAllEmpleadosAsync();

                empleados = empleados.Order().ToObservableCollection();

                var navParameter = new ShellNavigationQueryParameters
                {
                    { "ordenProduccionPt", ordenProduccionPt},
                    { "empleados", empleados}
                };
                await Shell.Current.GoToAsync("notificarPt", navParameter);
            }
            catch (Exception ex)
            {
                await Toast.Make(ex.Message).Show();
            }
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


        //private OrdenProduccion MapOrdenProduccion(OrdenProduccionDetalle detalle)
        //{
        //    return new OrdenProduccion
        //    {
        //        Centro = detalle.Cabecera.Centro,
        //        CodigoProduccion = detalle.Cabecera.CodigoProduccion,
        //        Orden = detalle.Cabecera.Orden,
        //        CodigoUsuario = SharedData.UsuarioSistema,
        //        Fecha = detalle.Cabecera.Fecha,
        //        Referencia = detalle.Cabecera.Referencia,
        //        Detalle = detalle.Detalle,
        //        CodigoMaterial = detalle.CodigoMaterial,
        //        CodigoProducto = detalle.CodigoProducto,
        //        Producto = detalle.Producto,
        //        CodigoUnidad = detalle.CodigoUnidad,
        //        Cantidad = detalle.Cantidad,
        //        Notificado = detalle.Saldo
        //    };
        //}


        private ObservableCollection<OrdenProduccionGroup> MapOrdenesProduccionGrouped(ObservableCollection<OrdenProduccion> ordenesPrd, ObservableCollection<OrdenProduccionPt> ordenesProducciondPt)
        {
            var ordenProducciondPtDic = ordenesProducciondPt
                .GroupBy(d => new
                {
                    d.Centro,
                    d.CodigoProduccion,
                    d.Orden
                }
                ).ToDictionary(g => g.Key, g => g.ToList());


            var ordenesProduccionGrouped = ordenesPrd
                .Select(ordenProduccion =>
                                {
                                    var key = new
                                    {
                                        ordenProduccion.Centro,
                                        ordenProduccion.CodigoProduccion,
                                        ordenProduccion.Orden
                                    };

                                    return new OrdenProduccionGroup(
                                        ordenProduccion,
                                        ordenProducciondPtDic.TryGetValue(key, out var ordenesProducciondPtGrouped) ? ordenesProducciondPtGrouped : new List<OrdenProduccionPt>()
                                    );
                                })
                .ToList();

            return new ObservableCollection<OrdenProduccionGroup>(ordenesProduccionGrouped);
        }


        internal async Task BackButtonPressed()
        {
            var res = await Shell.Current.DisplayAlert("Salir", "¿Desea cerrar sesión?", "Aceptar", "Cancelar");
            if (res)
            {
                await Shell.Current.GoToAsync("//login");
            }
        }




    }
}
