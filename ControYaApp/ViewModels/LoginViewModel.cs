﻿using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
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
            try
            {
                var ip = await _ipServidorRepo.GetIpServidorAsync();
                if (ip is null)
                {
                    var res = await Shell.Current.DisplayAlert("Alerta", "Primero debe registar la direccion IP del servidor", "Ir", "Cerrar");
                    if (res)
                    {
                        await Shell.Current.GoToAsync("config");
                        return;
                    }
                }

                var usuario = new Usuario
                {
                    NombreUsuario = SharedData.NombreUsuario,
                    Contrasena = Contrasena
                };

                var loadingPopUpp = new LoadingPopUp();
                _ = Shell.Current.CurrentPage.ShowPopupAsync(loadingPopUpp);

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
                await loadingPopUpp.CloseAsync();
            }
            catch (Exception ex)
            {
                await Toast.Make(ex.Message).Show();
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