using System.Collections.ObjectModel;
using System.ComponentModel;
using ControYaApp.Models;

namespace ControYaApp.Services.SharedData
{
    public interface ISharedData : INotifyPropertyChanged
    {
        string? NombreUsuario { get; set; }

        string? UsuarioSistema { get; set; }

        string? IpAddress { get; set; }

        string? Protocolo { get; set; }

        bool NotificarPtAuto { get; set; }

        ObservableCollection<OrdenProduccionGroup> AllOrdenesProduccionGroups { get; set; }

    }
}
