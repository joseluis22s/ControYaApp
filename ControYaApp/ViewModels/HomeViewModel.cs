using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using ControYaApp.Modelss;
using ControYaApp.Services.RestService;

namespace ControYaApp.ViewModels
{
    [QueryProperty(nameof(NombreUsuario), "NombreUsuario")]
    public partial class HomeViewModel : ViewModelBase
    {
        private string? _nombreUsuario;
        public string? NombreUsuario
        {
            get => _nombreUsuario;
            set
            {
                SetProperty(ref _nombreUsuario, value);
                // Aquí puedes asignar el valor al objeto Usuario
                //Usuario = new Usuario { NombreUsuario = value };
            }
        }


        private readonly RestService _restService;


        public ObservableCollection<PrdOrdenProduccion> OrdenesProduccion { get; set; } = [];



        //public Usuario Usuario
        //{
        //    get => _usuario;
        //    set 
        //    {
        //        SetProperty(ref _usuario, value)
        //    }
        //}



        public ICommand ObtenerPedidosCommand { get; }

        public HomeViewModel(RestService restService)
        {
            ObtenerPedidosCommand = new AsyncRelayCommand(ObtenerPedidosAsync);

            _restService = restService;
        }




        public async Task ObtenerPedidosAsync()
        {
        }
    }
}
