using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using ControYaApp.Models;
using ControYaApp.Services.AppLocalDatabase;
using ControYaApp.Services.Dialog;
using ControYaApp.Services.Navigation;
using ControYaApp.Services.SharedData;
using ControYaApp.ViewModels.Base;

namespace ControYaApp.ViewModels
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
                string ip = SharedData.Protocolo + SharedData.IpAddress;

                var res = await _dialogService.DisplayAlert("Guardar", "¿Desea guardar la configuración?", "Aceptar", "Cancelar");

                if (res)
                {
                    _dataConfigSave.Protocolo = SharedData.Protocolo;
                    _dataConfigSave.Ip = SharedData.IpAddress;
                    _dataConfigSave.AutoApproveProduccion = SharedData.AutoApproveProduccion;
                    _dataConfigSave.AutoApproveInventario = SharedData.AutoApproveInventario;
                    await _appDbReposService.DataConfigRepo.SaveIpServidor(_dataConfigSave);

                    _isSaved = true;

                    await _dialogService.ShowToast("Configuración guardada");
                    await NavigationService.GoBackAsync();
                }

            }
            catch (Exception ex)
            {
                await _dialogService.ShowToast(ex.Message);
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
                var res = await _dialogService.DisplayAlert("Alerta", "¿Está seguro que desea salir sin guardar una dirección IP?", "Aceptar", "Cancelar");
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
                var res = await _dialogService.DisplayAlert("Alerta", "¿Está seguro que desea salir de la configuración?", "Aceptar", "Cancelar");
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
