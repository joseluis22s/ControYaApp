using ControYaApp.ViewModels;

namespace ControYaApp.Services.SharedData
{
    public class SharedData : BaseViewModel, ISharedData
    {

        private string? _usuarioSistema;
        public string? UsuarioSistema
        {
            get => _usuarioSistema;
            set
            {
                SetProperty(ref _usuarioSistema, value);
            }
        }


        private string? _nombreUsuario;
        public string? NombreUsuario
        {
            get => _nombreUsuario;
            set
            {
                SetProperty(ref _nombreUsuario, value);
            }
        }


        private string? _ipServidor;
        public string? IpServidor
        {
            get => _ipServidor;
            set
            {
                SetProperty(ref _ipServidor, value);
            }
        }

    }
}
