﻿using System.Windows.Input;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Input;
using ControYaApp.Models;
using ControYaApp.Services.AppLocalDatabase;
using ControYaApp.Services.Dialog;
using ControYaApp.Services.Navigation;
using ControYaApp.Services.SharedData;
using ControYaApp.Services.WebService;
using ControYaApp.ViewModels.Base;

namespace ControYaApp.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly AppDbReposService _appDbReposService;


        public ISharedData SharedData { get; set; }



        private readonly RestService _restService;



        private string? _contrasena;
        public string? Contrasena
        {
            get => _contrasena;
            set => SetProperty(ref _contrasena, value);
        }



        public ICommand? GoToHomeCommand { get; }

        public ICommand? ProbarConexionCommand { get; }

        public ICommand GoToConfigCommand { get; }




        public LoginViewModel(INavigationService navigationService, IDialogService dialogService,
            AppDbReposService appDbReposService,
            RestService restService, ISharedData sharedData) : base(navigationService)
        {
            _dialogService = dialogService;
            _appDbReposService = appDbReposService;

            SharedData = sharedData;


            _restService = restService;


            GoToHomeCommand = new AsyncRelayCommand(GoToHomeAsync);
            GoToConfigCommand = new AsyncRelayCommand(GoToConfigAsync);
        }




        private async Task GoToConfigAsync()
        {
            await NavigationService.GoToAsync("config");
        }


        private async Task GoToHomeAsync()
        {
            _ = _dialogService.ShowLoadingPopUpAsync();

            try
            {
                var dataConfig = await _appDbReposService.DataConfigRepo.GetDataConfigAsync();
                if (dataConfig is null)
                {
                    var res = await _dialogService.DisplayAlertAsync("Alerta", "Primero debe registar la direccion IP del servidor", "Ir", "Cerrar");
                    if (res)
                    {
                        await NavigationService.GoToAsync("config");
                    }
                    return;
                }

                var usuario = new Usuario
                {
                    NombreUsuario = SharedData.NombreUsuario, // viene de la View
                    Contrasena = Contrasena
                };


                // TODO: Agregar que cuando no haya conexión, decir que se conecte en caso de que no se haya importado el usaurio.

                NetworkAccess accessType = Connectivity.Current.NetworkAccess;
                if (accessType != NetworkAccess.Internet)
                {
                    var usuarioDb = await _appDbReposService.UsuarioRepo.CheckUsuarioCredentialsAsync(usuario);
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
                await _dialogService.ShowToastAsync(ex.Message, ToastDuration.Long);
            }
            finally
            {
                await _dialogService.HideLoadingPopUpAsync();
            }
        }

        private async Task ValidateNavigationToHome(Usuario usuarioRes, Usuario usuario)
        {
            if ((usuarioRes is null) ||
                (usuarioRes.NombreUsuario != usuario.NombreUsuario && usuarioRes.Contrasena != usuario.Contrasena))
            {
                await _dialogService.ShowToastAsync("Usuario no encontrado o credenciales incorrectas", ToastDuration.Long);
            }
            else
            {
                SharedData.UsuarioSistema = usuarioRes.UsuarioSistema;
                await NavigationService.GoToAsync("//home");
            }
        }
    }
}