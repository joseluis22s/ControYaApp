﻿using System.Windows.Input;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ControYaApp.Services.DI;
using ControYaApp.Services.WebService;
using ControYaApp.Views.Controls;

namespace ControYaApp.ViewModels
{
    public partial class AppShellViewModel : BaseViewModel
    {

        private readonly RestService _restService;

        private readonly LocalRepoService _localRepoService;



        private bool _isConected;
        public bool IsConected
        {
            get => _isConected;
            set => SetProperty(ref _isConected, value);
        }



        public ICommand GoToLoginCommand { get; }

        public ICommand FlyoutShellCommand { get; }

        public ICommand ExtraerDatosCommand { get; }




        public AppShellViewModel(RestService restService, LocalRepoService localRepoService)
        {

            _restService = restService;
            _localRepoService = localRepoService;


            GoToLoginCommand = new AsyncRelayCommand(GoToLoginAsync);
            FlyoutShellCommand = new RelayCommand(FlyoutShell);
            ExtraerDatosCommand = new AsyncRelayCommand(ExtraerDatosAsync);
        }




        private async Task GoToLoginAsync()
        {
            await Shell.Current.GoToAsync("//login");

        }


        private void FlyoutShell()
        {
            Shell.Current.FlyoutIsPresented = !Shell.Current.FlyoutIsPresented;

            NetworkAccess accessType = Connectivity.Current.NetworkAccess;

            IsConected = false;
            if (accessType == NetworkAccess.Internet)
            {
                IsConected = true;
            }
        }


        private async Task ExtraerDatosAsync()
        {
            WeakReferenceMessenger.Default.Send(new ClearDataMessage("Vaciar"));

            var loadingPopUpp = new LoadingPopUp();
            _ = Shell.Current.CurrentPage.ShowPopupAsync(loadingPopUpp);

            await Shell.Current.DisplayAlert("¿Seguro que desea extraer datos?", "Se sobreescribiran los que están actualmente guardados", "Aceptar", "Cancelar");

            var usuarios = await _restService.GetAllUsuariosAsync();
            var ordenes = await _restService.GetOrdenesProduccionAsync();
            var periodos = await _restService.GetRangosPeriodos();
            var productos = await _restService.GetAllProductosTerminado();
            var materiales = await _restService.GetAllMaterialesEgreso();
            var empleados = await _restService.GetAllEmpleados();


            await _localRepoService.EmpleadosRepo.SaveAllEmpleadosAsync(empleados);
            await _localRepoService.MaterialEgresadoRepo.SaveAllEmAsync(materiales);
            await _localRepoService.ProductoTerminadoRepo.SaveAllPtAsync(productos);
            await _localRepoService.PeriodoRepo.SaveRangosPeriodosAsync(periodos);
            await _localRepoService.OrdenRepo.SaveAllOrdenesAsync(ordenes);
            await _localRepoService.UsuarioRepo.SaveAllUsuariosAsync(usuarios);

            Shell.Current.FlyoutIsPresented = false;

            await loadingPopUpp.CloseAsync();
        }




    }
}
