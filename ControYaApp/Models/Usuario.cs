using ControYaApp.ViewModels;

namespace ControYaApp.Models
{
    public partial class Usuario : ViewModelBase
    {
        //private string? _nombreUsuario;
        //public string? _contrasena;

        //public string? NombreUsuario
        //{
        //    get => _nombreUsuario;
        //    set => SetProperty(ref _nombreUsuario, value);
        //}
        //public string? Contrasena
        //{
        //    get => _contrasena;
        //    set => SetProperty(ref _contrasena, value);
        //}

        public string? NombreUsuario { get; set; }

        public string? Contrasena { get; set; }

    }
}
