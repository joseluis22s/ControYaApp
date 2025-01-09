using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using ControYaApp.Models;

namespace ControYaApp.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
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


        public ICommand? GoToHomeCommand { get; }

        public ICommand? ContrasenaVisibleCommand { get; }


        public LoginViewModel()
        {
            EsVisibleContrasena = true;
            //_databaseConnection = databaseConnection;
            GoToHomeCommand = new AsyncRelayCommand(GoToHome);
            ContrasenaVisibleCommand = new Command(EstadoEsVisibleContrasena);
            //VerificarConexionDatabase().GetAwaiter();
        }

        private async Task GoToHome()
        {
            await Shell.Current.GoToAsync("//home");
        }


        private void EstadoEsVisibleContrasena()
        {
            EsVisibleContrasena = !EsVisibleContrasena;
            NoEsVisibleContrasena = EsVisibleContrasena;
        }

        private void VerificarCamposVacios()
        {

        }


        private async Task VerificarConexionDatabase()
        {
            //bool estaConectado = _databaseConnection.ConectarDatabase();
            //if (estaConectado)
            //{
            //    await Toast.Make("Conexión a la base de datos exitosa.").Show();
            //}
            //else
            //{
            //    await Toast.Make("No se pudo conectar la base de datos.").Show();
            //}
        }
    }
}
