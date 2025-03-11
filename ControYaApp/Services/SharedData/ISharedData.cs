using System.ComponentModel;

namespace ControYaApp.Services.SharedData
{
    public interface ISharedData : INotifyPropertyChanged
    {
        string? NombreUsuario { get; set; }

        string? UsuarioSistema { get; set; }

        string? IpServidor { get; set; }
    }
}
