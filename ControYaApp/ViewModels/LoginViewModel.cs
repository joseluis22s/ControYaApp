using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using ControYaApp.Models;

namespace ControYaApp.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
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


        public LoginViewModel()
        {
            EsVisibleContrasena = true;
            NoEsVisibleContrasena = false;
            GoToHomeCommand = new AsyncRelayCommand(GoToHome);
            ContrasenaVisibleCommand = new RelayCommand(EstadoEsVisibleContrasena);
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
    }
}
