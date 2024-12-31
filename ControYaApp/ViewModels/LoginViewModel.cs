
using System.Windows.Input;
using ControYaApp.Models;

namespace ControYaApp.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private Usuario? _usuario;

        public Usuario? Usuario
        {
            get => _usuario;
            set => SetProperty(ref _usuario, value);
        }

        private bool _esVisibleContrasena;
        public bool EsVisibleContrasena
        {
            get => _esVisibleContrasena;
            set => SetProperty(ref _esVisibleContrasena, value);
        }

        private bool _noEsVisibleContrasena;

        public bool NoEsVisibleContrasena
        {
            get => _noEsVisibleContrasena;
            set => SetProperty(ref _noEsVisibleContrasena, value);
        }

        public ICommand GoToHomeCommand { get; private set; }
        public ICommand ContrasenaVisibleCommand { get; private set; }

        // TODO: REVISAR SI EL DATABINDING EN LA VISTA  NECESITA UN CONSTRUCTOR SIN ARGUMENTOS DEL VIEWMODEL
        public LoginViewModel()
        {
        }
        public LoginViewModel(Usuario usuario)
        {
            GoToHomeCommand = GoToHomeAsync();
            ContrasenaVisibleCommand = new Command(EstadoEsVisibleContrasena);
        }

        private Command GoToHomeAsync()
        {
            EsVisibleContrasena = false;

            return new Command(async () =>
            {
                await Shell.Current.GoToAsync("//home");
            });
        }

        private void VerificarConexionDatabase()
        {
            // TODO: VERIFICAR SI SE LOGRO VERIFICAR A LA BASE DE DATOS 
        }


        private void EstadoEsVisibleContrasena()
        {
            EsVisibleContrasena = !EsVisibleContrasena;
            NoEsVisibleContrasena = EsVisibleContrasena;
        }
    }
}
