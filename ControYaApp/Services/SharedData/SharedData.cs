using System.Collections.ObjectModel;
using ControYaApp.Models;
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

        private string? _protocolo;
        public string? Protocolo
        {
            get => _protocolo;
            set
            {
                SetProperty(ref _protocolo, value);
            }
        }


        private string? _ipAddress;
        public string? IpAddress
        {
            get => _ipAddress;
            set
            {
                SetProperty(ref _ipAddress, value);
            }
        }


        private bool _authorizedNotification;
        public bool AuthorizedNotification
        {
            get => _authorizedNotification;
            set
            {
                SetProperty(ref _authorizedNotification, value);
            }
        }

        public ObservableCollection<OrdenProduccionGroup> AllOrdenesProduccionGroups { get; set; }

    }
}
