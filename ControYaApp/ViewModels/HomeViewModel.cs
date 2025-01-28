using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using ControYaApp.Models;
using ControYaApp.Services.RestService;
using ControYaApp.Views.Controls;

namespace ControYaApp.ViewModels
{
    [QueryProperty(nameof(NombreUsuario), "NombreUsuario")]
    public partial class HomeViewModel : ViewModelBase
    {

        private readonly RestService _restService;


        private string? _nombreUsuario;

        private ObservableCollection<OrdenProduccion>? _prdOrdenesProduccion;


        public string? NombreUsuario
        {
            get => _nombreUsuario;
            set
            {
                SetProperty(ref _nombreUsuario, value);
            }
        }

        public ObservableCollection<OrdenProduccion>? OrdenesProduccion
        {
            get => _prdOrdenesProduccion;
            set => SetProperty(ref _prdOrdenesProduccion, value);
        }



        public ICommand ObtenerPedidosCommand { get; }

        public HomeViewModel(RestService restService)
        {
            var fecha = DateTime.Now;
            ObtenerPedidosCommand = new AsyncRelayCommand(ObtenerPedidosAsync);

            _restService = restService;

        }

        public async Task ObtenerPedidosAsync()
        {
            var loadingPopUpp = new LoadingPopUp();
            _ = Shell.Current.CurrentPage.ShowPopupAsync(loadingPopUpp);

            var listaOrdenes = await _restService.GetAllPrdOrdenesProduccionAsync(NombreUsuario);
            if (listaOrdenes.Count == 0)
            {
                await Toast.Make("No se han cargado órdenes").Show();
            }

            OrdenesProduccion = listaOrdenes;

            await loadingPopUpp.CloseAsync();

        }
    }
}
