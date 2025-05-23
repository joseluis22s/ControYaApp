﻿using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using ControYaApp.Models;
using ControYaApp.Services.AppLocalDatabase;
using ControYaApp.Services.Dialog;
using ControYaApp.Services.Navigation;
using ControYaApp.Services.SharedData;
using ControYaApp.ViewModels.Base;

namespace CbMovil.ViewModels
{
    public partial class ConfigViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly AppDbReposService _appDbReposService;



        private bool _isSaved;

        private DataConfig _dataConfigSave = new();





        public ISharedData SharedData { get; set; }



        public ICommand SaveIpServidorCommand { get; }

        public ICommand BackButtonPressedCommand { get; }

        public ICommand SelectedItemChangedCommand { get; }



        public ConfigViewModel(INavigationService navigationServie, IDialogService dialogService,
            AppDbReposService appDbReposService, ISharedData sharedData) : base(navigationServie)
        {
            _dialogService = dialogService;
            _appDbReposService = appDbReposService;


            SharedData = sharedData; //No mover.


            InitData();

            SaveIpServidorCommand = new AsyncRelayCommand(SaveIpServidorAsync);
            BackButtonPressedCommand = new AsyncRelayCommand(BackButtonPressed);
            SelectedItemChangedCommand = new RelayCommand<string>(SelectedItemChanged);

        }

        private void InitData()
        {
            _dataConfigSave.Protocolo = SharedData.Protocolo;
            _dataConfigSave.Ip = SharedData.IpAddress;
            _dataConfigSave.AutoApproveProduccion = SharedData.AutoApproveProduccion;
            _dataConfigSave.AutoApproveInventario = SharedData.AutoApproveInventario;
            _dataConfigSave.EnableLotes = SharedData.EnableLotes;
        }

        private async void SelectedItemChanged(string selectedItem)
        {
            _dataConfigSave.Protocolo = SharedData.Protocolo;
            SharedData.Protocolo = selectedItem;
        }


        private async Task SaveIpServidorAsync()
        {
            try
            {

                var res = await _dialogService.DisplayAlertAsync("Guardar", "¿Desea guardar la configuración?", "Aceptar", "Cancelar");

                if (res)
                {
                    _dataConfigSave.Protocolo = SharedData.Protocolo;
                    _dataConfigSave.Ip = SharedData.IpAddress;
                    _dataConfigSave.AutoApproveProduccion = SharedData.AutoApproveProduccion;
                    _dataConfigSave.AutoApproveInventario = SharedData.AutoApproveInventario;
                    _dataConfigSave.EnableLotes = SharedData.EnableLotes;
                    await _appDbReposService.DataConfigRepo.SaveIpServidor(_dataConfigSave);

                    _isSaved = true;

                    await _dialogService.ShowToastAsync("Configuración guardada");
                    await NavigationService.GoBackAsync();
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowToastAsync(ex.Message);
            }
        }


        internal async Task BackButtonPressed()
        {
            if (_isSaved)
            {
                await NavigationService.GoBackAsync();
                return;
            }

            if (string.IsNullOrEmpty(SharedData.IpAddress))
            {
                var res = await _dialogService.DisplayAlertAsync("Alerta", "¿Está seguro que desea salir sin guardar una dirección IP?", "Aceptar", "Cancelar");
                if (res)
                {
                    SharedData.IpAddress = _dataConfigSave.Ip;
                    SharedData.Protocolo = _dataConfigSave.Protocolo;
                    await NavigationService.GoBackAsync();
                    return;
                }
            }
            else
            {
                var res = await _dialogService.DisplayAlertAsync("Alerta", "¿Está seguro que desea salir de la configuración?", "Aceptar", "Cancelar");
                if (res)
                {
                    SharedData.IpAddress = _dataConfigSave.Ip;
                    SharedData.Protocolo = _dataConfigSave.Protocolo;
                    await NavigationService.GoBackAsync();
                }
            }
        }
    }
}
