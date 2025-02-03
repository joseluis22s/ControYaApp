using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using ControYaApp.Models;
using ControYaApp.Services.LocalDatabase.Repositories;
using ControYaApp.Services.WebService;
using ControYaApp.Views.Controls;

namespace ControYaApp.ViewModels
{
    public partial class LoginViewModel : ViewModelBase
    {

        private readonly RestService _restService;

        private readonly UsuarioRepo _usuarioRepo;


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



        public ICommand? GoToOrdenesCommand { get; }

        public ICommand? ContrasenaVisibleCommand { get; }

        public ICommand? ProbarConexionCommand { get; }


        public LoginViewModel(UsuarioRepo usuarioRepo, RestService restService)
        {
            EsVisibleContrasena = true;
            NoEsVisibleContrasena = false;
            GoToOrdenesCommand = new AsyncRelayCommand(GoToOrdenesAsync);
            ContrasenaVisibleCommand = new RelayCommand(EstadoEsVisibleContrasena);

            _restService = restService;
            _usuarioRepo = usuarioRepo;
        }

        private async Task GoToOrdenesAsync()
        {
            if (string.IsNullOrEmpty(Usuario?.NombreUsuario) || string.IsNullOrEmpty(Usuario?.Contrasena))
            {
                await Toast.Make("Los campos no seben estar vacios.").Show();
            }
            else
            {
                var usuario = new Usuario
                {
                    NombreUsuario = Usuario.NombreUsuario,
                    Contrasena = Usuario?.Contrasena
                };

                var loadingPopUpp = new LoadingPopUp();
                _ = Shell.Current.CurrentPage.ShowPopupAsync(loadingPopUpp);

                NetworkAccess accessType = Connectivity.Current.NetworkAccess;
                // TODO: Agregar que cuando no haya conexión, decir que se conecte en caso de que no se haya importado el usaurio.
                try
                {
                    var result = await _usuarioRepo.CheckUsuarioCredentialsAsync(usuario);

                    if (result.TryGetValue("usuarioSistema", out object? usuarioSistema) &&
                        result.TryGetValue("estaRegistrado", out object? estaRegistrado))
                    {
                        var estado1 = bool.Parse(estaRegistrado.ToString());
                        if (estado1)
                        {
                            usuario.UsuarioSistema = usuarioSistema.ToString();

                            var navParameter = new ShellNavigationQueryParameters { { "usuario", usuario } };

                            await Shell.Current.GoToAsync("//ordenes", navParameter);
                        }
                        else if (accessType == NetworkAccess.Internet)
                        {
                            var res = await _restService.CheckUsuarioCredentialsAsync(Usuario);

                            if (res.TryGetValue("estaRegistrado", out object? estaRegistrado1) &&
                                res.TryGetValue("usuarioSistema", out object? usuarioSistema1))
                            {

                                var estado2 = bool.Parse(estaRegistrado1.ToString());
                                if (estado2)
                                {
                                    usuario.UsuarioSistema = usuarioSistema1.ToString();

                                    var navParameter = new ShellNavigationQueryParameters { { "usuario", usuario } };

                                    await Shell.Current.GoToAsync("//ordenes", navParameter);
                                }
                                else
                                {
                                    await Toast.Make("Credenciales incorrectas").Show();
                                }
                            }
                            else
                            {
                                await Toast.Make("Error de sistema").Show();
                            }
                        }
                        else
                        {
                            await Toast.Make("Usuario no encontrado").Show();
                        }
                    }
                    else
                    {
                        await Toast.Make("Error de sistema").Show();
                    }
                }
                catch (Exception ex)
                {
                    await Toast.Make(ex.Message).Show();
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


    }
}
