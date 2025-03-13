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

        private IpServidor _ipServidorSave = new();


        private readonly IpServidorRepo _ipServidorRepo;



        public ISharedData SharedData { get; set; }



        public ICommand SaveIpServidorCommand { get; }

        public ICommand BackButtonPressedCommand { get; }

        public ICommand SelectedItemChangedCommand { get; }



        public ConfigViewModel(IpServidorRepo ipServidorRepo, ISharedData sharedData)
        {
            SharedData = sharedData; //No mover.

            _ipServidorRepo = ipServidorRepo;

            InitData();

            SaveIpServidorCommand = new AsyncRelayCommand(SaveIpServidorAsync);
            BackButtonPressedCommand = new AsyncRelayCommand(BackButtonPressed);
            SelectedItemChangedCommand = new RelayCommand<string>(SelectedItemChanged);

        }

        private void InitData()
        {
            _ipServidorSave.Protocolo = SharedData.Protocolo;
            _ipServidorSave.Ip = SharedData.IpAddress;
        }

        private async void SelectedItemChanged(string selectedItem)
        {
            _ipServidorSave.Protocolo = SharedData.Protocolo;
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
                    _ipServidorSave.Protocolo = SharedData.Protocolo;
                    _ipServidorSave.Ip = SharedData.IpAddress;
                    await _ipServidorRepo.SaveIpServidor(_ipServidorSave);

                    _isSaved = true;

                    await Toast.Make("Dirección IP guardada").Show();
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
                    SharedData.IpAddress = _ipServidorSave.Ip;
                    SharedData.Protocolo = _ipServidorSave.Protocolo;
                    await Shell.Current.GoToAsync("..");
                    return;
                }
            }

            if (_ipServidorSave.Ip != SharedData.IpAddress)
            {
                var res = await Shell.Current.DisplayAlert("Alerta", "¿Está seguro que desea salir sin guardar los cambios?", "Aceptar", "Cancelar");
                if (res)
                {
                    SharedData.IpAddress = _ipServidorSave.Ip;
                    SharedData.Protocolo = _ipServidorSave.Protocolo;
                    await Shell.Current.GoToAsync("..");
                    return;
                }
            }
            else
            {
                var res = await Shell.Current.DisplayAlert("Alerta", "¿Está seguro que desea salir de la configuración?", "Aceptar", "Cancelar");
                if (res)
                {
                    SharedData.IpAddress = _ipServidorSave.Ip;
                    SharedData.Protocolo = _ipServidorSave.Protocolo;
                    await Shell.Current.GoToAsync("..");
                }
            }
        }




    }
}
