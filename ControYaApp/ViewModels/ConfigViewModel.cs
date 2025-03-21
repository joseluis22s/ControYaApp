using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.Input;
using ControYaApp.Models;
using ControYaApp.Services.LocalDatabase.Repositories;
using ControYaApp.Services.SharedData;

namespace ControYaApp.ViewModels
{
    public class ConfigViewModel : BaseViewModel
    {
        private bool _isSaved;

        private DataConfig _dataConfigSave = new();


        private readonly DataConfigRepo _dataConfigRepo;



        public ISharedData SharedData { get; set; }



        public ICommand SaveIpServidorCommand { get; }

        public ICommand BackButtonPressedCommand { get; }

        public ICommand SelectedItemChangedCommand { get; }



        public ConfigViewModel(DataConfigRepo dataConfigRepo, ISharedData sharedData)
        {
            SharedData = sharedData; //No mover.

            _dataConfigRepo = dataConfigRepo;

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

                var res = await Shell.Current.DisplayAlert("Nueva conexión", $"¿Está seguro que desea guardar la siguiente ip?:\n{ip}", "Aceptar", "Cancelar");

                if (res)
                {
                    _dataConfigSave.Protocolo = SharedData.Protocolo;
                    _dataConfigSave.Ip = SharedData.IpAddress;
                    _dataConfigSave.AutoApproveProduccion = SharedData.AutoApproveProduccion;
                    _dataConfigSave.AutoApproveInventario = SharedData.AutoApproveInventario;
                    await _dataConfigRepo.SaveIpServidor(_dataConfigSave);

                    _isSaved = true;

                    await Toast.Make("Configuración guardada").Show();
                    await Shell.Current.GoToAsync("..");
                }

            }
            catch (Exception ex)
            {
                await Toast.Make(ex.Message).Show();
            }
        }


        internal async Task BackButtonPressed()
        {
            if (_isSaved)
            {
                await Shell.Current.GoToAsync("..");
                return;
            }

            if (string.IsNullOrEmpty(SharedData.IpAddress))
            {
                var res = await Shell.Current.DisplayAlert("Alerta", "¿Está seguro que desea salir sin guardar una dirección IP?", "Aceptar", "Cancelar");
                if (res)
                {
                    SharedData.IpAddress = _dataConfigSave.Ip;
                    SharedData.Protocolo = _dataConfigSave.Protocolo;
                    SharedData.AutoApproveProduccion = _dataConfigSave.AutoApproveProduccion;
                    SharedData.AutoApproveInventario = _dataConfigSave.AutoApproveInventario;
                    await Shell.Current.GoToAsync("..");
                    return;
                }
            }

            if (_dataConfigSave.Ip != SharedData.IpAddress)
            {
                var res = await Shell.Current.DisplayAlert("Alerta", "¿Está seguro que desea salir sin guardar los cambios?", "Aceptar", "Cancelar");
                if (res)
                {
                    SharedData.IpAddress = _dataConfigSave.Ip;
                    SharedData.Protocolo = _dataConfigSave.Protocolo;
                    SharedData.AutoApproveProduccion = _dataConfigSave.AutoApproveProduccion;
                    SharedData.AutoApproveInventario = _dataConfigSave.AutoApproveInventario;
                    await Shell.Current.GoToAsync("..");
                    return;
                }
            }
            else
            {
                var res = await Shell.Current.DisplayAlert("Alerta", "¿Está seguro que desea salir de la configuración?", "Aceptar", "Cancelar");
                if (res)
                {
                    SharedData.IpAddress = _dataConfigSave.Ip;
                    SharedData.Protocolo = _dataConfigSave.Protocolo;
                    SharedData.AutoApproveProduccion = _dataConfigSave.AutoApproveProduccion;
                    SharedData.AutoApproveInventario = _dataConfigSave.AutoApproveInventario;
                    await Shell.Current.GoToAsync("..");
                }
            }
        }




    }
}
