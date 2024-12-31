using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.Input;
using ControYaApp.Models;
using ControYaApp.Services.Database;

namespace ControYaApp.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {

        private readonly DatabaseConnection _databaseConnection;

        private Usuario _usuario;

        private bool _esVisibleContrasena;

        private bool _noEsVisibleContrasena;

        public Usuario Usuario
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

        public ICommand? GoToHomeCommand { get; private set; }

        public ICommand? ContrasenaVisibleCommand { get; private set; }


        public LoginViewModel(DatabaseConnection databaseConnection)
        {
            EsVisibleContrasena = true;
            _databaseConnection = databaseConnection;
            GoToHomeCommand = new AsyncRelayCommand(GoToHomeAsync);
            ContrasenaVisibleCommand = new Command(EstadoEsVisibleContrasena);
            //VerificarConexionDatabase().GetAwaiter();
        }

        private async Task GoToHomeAsync()
        {
            await Shell.Current.GoToAsync("//home");
        }

        private async Task VerificarConexionDatabase()
        {
            bool estaConectado = _databaseConnection.ConectarDatabase();
            if (estaConectado)
            {
                await Toast.Make("Conexión a la base de datos exitosa.").Show();
            }
            else
            {
                await Toast.Make("No se pudo conectar la base de datos.").Show();
            }
        }


        private void EstadoEsVisibleContrasena()
        {
            EsVisibleContrasena = !EsVisibleContrasena;
            NoEsVisibleContrasena = EsVisibleContrasena;
        }
    }
}
