using ControYaApp.ViewModels;

namespace ControYaApp.Models
{
    public class Usuario : ViewModelBase
    {
        private string? _nombreUsuario;
        private string? _contrasena;

        public string? NombreUsuario
        {
            get => _nombreUsuario;
            set => SetProperty(ref _nombreUsuario, value);
        }
        public string? Contrasena
        {
            get => _contrasena;
            set => SetProperty(ref _contrasena, value);
        }

    }
}
