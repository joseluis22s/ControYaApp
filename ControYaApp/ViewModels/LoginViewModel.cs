using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using ControYaApp.Models;
using ControYaApp.Services.LocalDatabase.Repositories;
using ControYaApp.Services.SharedData;
using ControYaApp.Services.WebService;
using ControYaApp.Views.Controls;

namespace ControYaApp.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {

        public ISharedData SharedData { get; set; }



        private readonly RestService _restService;

        private readonly UsuarioRepo _usuarioRepo;

        private readonly IpServidorRepo _ipServidorRepo;



        private string? _contrasena;
        public string? Contrasena
        {
            get => _contrasena;
            set => SetProperty(ref _contrasena, value);
        }



        public ICommand? GoToHomeCommand { get; }

        public ICommand? ProbarConexionCommand { get; }

        public ICommand GoToConfigCommand { get; }




        public LoginViewModel(UsuarioRepo usuarioRepo, RestService restService, IpServidorRepo ipServidorRepo, ISharedData sharedData)
        {

            SharedData = sharedData;


            _restService = restService;
            _usuarioRepo = usuarioRepo;
            _ipServidorRepo = ipServidorRepo;


            GoToHomeCommand = new AsyncRelayCommand(GoToHomeAsync);
            GoToConfigCommand = new AsyncRelayCommand(GoToConfigAsync);

        }




        private async Task GoToConfigAsync()
        {
            await Shell.Current.GoToAsync("config");
        }


        private async Task GoToHomeAsync()
        {
            if (string.IsNullOrEmpty(SharedData.NombreUsuario) || string.IsNullOrEmpty(Contrasena))
            {
                await Toast.Make("Los campos no seben estar vacios.").Show();
            }
            else
            {
                var ip = await _ipServidorRepo.GetIpServidorAsync();
                if (ip is null)
                {

                    var res = await Shell.Current.DisplayAlert("Alerta", "Primero debe registar la direccion IP del servidor", "Ir", "Cerrar");
                    if (res)
                    {
                        await Shell.Current.GoToAsync("config");
                    }
                }
                else
                {
                    var usuario = new Usuario
                    {
                        NombreUsuario = SharedData.NombreUsuario,
                        Contrasena = Contrasena
                    };

                    var loadingPopUpp = new LoadingPopUp();
                    _ = Shell.Current.CurrentPage.ShowPopupAsync(loadingPopUpp);

                    NetworkAccess accessType = Connectivity.Current.NetworkAccess;
                    // TODO: Agregar que cuando no haya conexión, decir que se conecte en caso de que no se haya importado el usaurio.
                    try
                    {
                        // TODO: VER MENSAJE de db
                        var result = await _usuarioRepo.CheckUsuarioCredentialsAsync(usuario);

                        if (result.TryGetValue("usuarioSistema", out object? usuarioSistema) &&
                            result.TryGetValue("estaRegistrado", out object? estaRegistrado))
                        {
                            var estado1 = bool.Parse(estaRegistrado.ToString());
                            if (estado1)
                            {
                                SharedData.UsuarioSistema = usuarioSistema.ToString();

                                await Shell.Current.GoToAsync("//home");
                            }
                            else if (accessType == NetworkAccess.Internet)
                            {
                                var res = await _restService.CheckUsuarioCredentialsAsync(usuario);

                                if (res.TryGetValue("estaRegistrado", out object? estaRegistrado1) &&
                                    res.TryGetValue("usuarioSistema", out object? usuarioSistema1) &&
                                        estaRegistrado1 != null)
                                {

                                    var estado2 = bool.Parse(estaRegistrado1.ToString());
                                    if (estado2)
                                    {
                                        SharedData.UsuarioSistema = usuarioSistema.ToString();
                                        await Shell.Current.GoToAsync("//home");
                                    }
                                    else
                                    {
                                        await Toast.Make(usuarioSistema1.ToString()).Show();
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
        }




    }
}
