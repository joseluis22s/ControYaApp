using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using ControYaApp.Models;
using ControYaApp.Services.LocalDatabase.Repositories;
using ControYaApp.Services.SharedData;
using ControYaApp.Services.WebService;
using ControYaApp.ViewModels.Base;
using ControYaApp.Views.Controls;

namespace ControYaApp.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {

        public ISharedData SharedData { get; set; }



        private readonly RestService _restService;

        private readonly UsuarioRepo _usuarioRepo;

        private readonly DataConfigRepo _dataConfigRepo;



        private string? _contrasena;
        public string? Contrasena
        {
            get => _contrasena;
            set => SetProperty(ref _contrasena, value);
        }



        public ICommand? GoToHomeCommand { get; }

        public ICommand? ProbarConexionCommand { get; }

        public ICommand GoToConfigCommand { get; }




        public LoginViewModel(UsuarioRepo usuarioRepo, RestService restService, DataConfigRepo dataConfigRepo, ISharedData sharedData)
        {

            SharedData = sharedData;


            _restService = restService;
            _usuarioRepo = usuarioRepo;
            _dataConfigRepo = dataConfigRepo;


            GoToHomeCommand = new AsyncRelayCommand(GoToHomeAsync);
            GoToConfigCommand = new AsyncRelayCommand(GoToConfigAsync);

            // TODO: Eliminar el siguiente objeto.
            var mockUsuario = new Usuario
            {
                NombreUsuario = SharedData.NombreUsuario = "jadame",
                Contrasena = Contrasena = "admin123"
            };
            // TODO: Fin eliminar objeto.
        }




        private async Task GoToConfigAsync()
        {
            await Shell.Current.GoToAsync("config");
        }


        private async Task GoToHomeAsync()
        {
            var loadingPopUpp = new LoadingPopUp();
            _ = Shell.Current.CurrentPage.ShowPopupAsync(loadingPopUpp);

            try
            {
                var dataConfig = await _dataConfigRepo.GetDataConfigAsync();
                if (dataConfig is null)
                {
                    var res = await Shell.Current.DisplayAlert("Alerta", "Primero debe registar la direccion IP del servidor", "Ir", "Cerrar");
                    if (res)
                    {
                        await Shell.Current.GoToAsync("config");
                    }
                    return;
                }

                var usuario = new Usuario
                {
                    NombreUsuario = SharedData.NombreUsuario,
                    Contrasena = Contrasena
                };


                // TODO: Agregar que cuando no haya conexión, decir que se conecte en caso de que no se haya importado el usaurio.

                NetworkAccess accessType = Connectivity.Current.NetworkAccess;
                if (accessType != NetworkAccess.Internet)
                {
                    var usuarioDb = await _usuarioRepo.CheckUsuarioCredentialsAsync(usuario);
                    await ValidateNavigationToHome(usuarioDb, usuario);
                }
                else
                {
                    var usuarioWs = await _restService.CheckUsuarioCredentialsAsync(usuario);
                    await ValidateNavigationToHome(usuarioWs, usuario);
                }
            }
            catch (Exception ex)
            {
                await Toast.Make(ex.Message, ToastDuration.Long).Show();
            }
            finally
            {
                await loadingPopUpp.CloseAsync();
            }
        }

        private async Task ValidateNavigationToHome(Usuario usuarioRes, Usuario usuario)
        {
            if ((usuarioRes is null) ||
                (usuarioRes.NombreUsuario != usuario.NombreUsuario && usuarioRes.Contrasena != usuario.Contrasena))
            {
                await Toast.Make("Usuario no encontrado o credenciales incorrectas", ToastDuration.Long).Show();
            }
            else
            {
                SharedData.UsuarioSistema = usuarioRes.UsuarioSistema;
                await Shell.Current.GoToAsync("//home");
            }
        }
    }
}