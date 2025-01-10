using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Input;
using ControYaApp.Models;
using ControYaApp.Services.Database;
using ControYaApp.ViewModels.Controls;

namespace ControYaApp.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly DatabaseConnection _databaseConnection;

        private readonly IPopupService _popupService;

        private Usuario? _usuario;

        private bool _esVisibleContrasena;

        private bool _noEsVisibleContrasena;


        public Usuario? Usuario
        {
            get => _usuario;
            set => SetProperty(ref _usuario, value);
        }

        public bool EsVisibleContrasena
        {
            get => _esVisibleContrasena;
            set => SetProperty(ref _esVisibleContrasena, value);
        }

        public bool NoEsVisibleContrasena
        {
            get => _noEsVisibleContrasena;
            set => SetProperty(ref _noEsVisibleContrasena, value);
        }


        public ICommand? GoToHomeCommand { get; }

        public ICommand? ContrasenaVisibleCommand { get; }

        public ICommand? ProbarConexionCommand { get; }


        public LoginViewModel(DatabaseConnection databaseConnection, IPopupService popupService)
        {
            _popupService = popupService;
            _databaseConnection = databaseConnection;
            EsVisibleContrasena = true;
            NoEsVisibleContrasena = false;
            GoToHomeCommand = new AsyncRelayCommand(GoToHome);
            ContrasenaVisibleCommand = new RelayCommand(EstadoEsVisibleContrasena);
            ProbarConexionCommand = new AsyncRelayCommand(ProbarConexion);
        }

        private async Task GoToHome()
        {
            await Shell.Current.GoToAsync("//home");
        }

        private void EstadoEsVisibleContrasena()
        {
            if (EsVisibleContrasena)
            {
                EsVisibleContrasena = false;
                NoEsVisibleContrasena = true;
            }
            else
            {
                EsVisibleContrasena = true;
                NoEsVisibleContrasena = false;
            }
        }

        private async Task ProbarConexion()
        {

            if (_databaseConnection.ConectarDatabase())
            {
                Toast.Make("Se ha conectado");
            }
            else
            {
                Toast.Make("Error al conectar");
            }

            await _popupService.ShowPopupAsync<LoadingPopUpViewModel>();
            await _popupService.ClosePopupAsync();
        }
    }
}
