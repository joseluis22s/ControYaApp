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


        public string? Protocolo { get; set; }


        public string? IpAddress { get; set; }


        private bool _notificarPtAuto;
        public bool NotificarPtAuto
        {
            get => _notificarPtAuto;
            set
            {
                SetProperty(ref _notificarPtAuto, value);
            }
        }

    }
}
