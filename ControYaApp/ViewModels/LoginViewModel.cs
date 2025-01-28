using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using ControYaApp.Models;
using ControYaApp.Services.Database;
using ControYaApp.Services.RestService;
using ControYaApp.Views.Controls;

namespace ControYaApp.ViewModels
{
    public partial class LoginViewModel : ViewModelBase
    {

        private readonly RestService _restService;

        private readonly DatabaseConnection _databaseConnection;


        private Usuario? _usuario = new();

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


        public LoginViewModel(DatabaseConnection databaseConnection, RestService restService)
        {
            EsVisibleContrasena = true;
            NoEsVisibleContrasena = false;
            GoToHomeCommand = new AsyncRelayCommand(GoToHomeAsync);
            ContrasenaVisibleCommand = new RelayCommand(EstadoEsVisibleContrasena);
            ProbarConexionCommand = new AsyncRelayCommand(ProbarConexionAsync);

            _restService = restService;
            _databaseConnection = databaseConnection;
        }

        private async Task GoToHomeAsync()
        {
            if (string.IsNullOrEmpty(Usuario?.NombreUsuario) || string.IsNullOrEmpty(Usuario?.Contrasena))
            {
                await Toast.Make("Los campos no seben estar vacios.").Show();
            }
            else
            {

                var loadingPopUpp = new LoadingPopUp();
                _ = Shell.Current.CurrentPage.ShowPopupAsync(loadingPopUpp);

                bool res = await _restService.VerificarCredencialesUsuario(Usuario);
                if (res)
                {
                    var navParameter = new ShellNavigationQueryParameters
                    {
                        {"NombreUsuario", Usuario.NombreUsuario}
                    };
                    await Shell.Current.GoToAsync("//home", navParameter);
                }
                else
                {
                    await Toast.Make("Error al iniciar sesión").Show();
                }

                await loadingPopUpp.CloseAsync();
            }
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

        private async Task ProbarConexionAsync()
        {
            var loadingPopUpp = new LoadingPopUp();
            _ = Shell.Current.CurrentPage.ShowPopupAsync(loadingPopUpp);

            bool estaConectado = await _databaseConnection.ConectarDatabase();

            if (estaConectado)
            {
                await Toast.Make("Se ha conectado").Show();
            }
            else
            {
                await Toast.Make("Error al conectar").Show();
            }
            await loadingPopUpp.CloseAsync();
        }
    }
}
