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
        private string _ip;

        private bool _isSaved;



        private readonly IpServidorRepo _ipServidorRepo;



        public ISharedData SharedData { get; set; }



        public ICommand SaveIpServidorCommand { get; }

        public ICommand BackButtonPressedCommand { get; }

        public ICommand SelectedItemChangedCommand { get; }



        public ConfigViewModel(IpServidorRepo ipServidorRepo, ISharedData sharedData)
        {
            SharedData = sharedData; //No mover.

            _ipServidorRepo = ipServidorRepo;

            _ip = SharedData.IpAddress;

            SaveIpServidorCommand = new AsyncRelayCommand(SaveIpServidorAsync);
            BackButtonPressedCommand = new AsyncRelayCommand(BackButtonPressed);
            SelectedItemChangedCommand = new RelayCommand<string>(SelectedItemChanged);

        }

        private async void SelectedItemChanged(string selectedItem)
        {
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
                    IpServidor ipServidor = new()
                    {
                        Protocolo = SharedData.Protocolo,
                        Ip = SharedData.IpAddress
                    };
                    await _ipServidorRepo.SaveIpServidor(ipServidor);

                    _isSaved = true;
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
                    await Shell.Current.GoToAsync("..");
                    return;
                }
            }

            if (_ip != SharedData.IpAddress)
            {
                var res = await Shell.Current.DisplayAlert("Alerta", "¿Está seguro que desea salir sin guardar los cambios?", "Aceptar", "Cancelar");
                if (res)
                {
                    await Shell.Current.GoToAsync("..");
                    return;
                }
            }
            else
            {
                var res = await Shell.Current.DisplayAlert("Alerta", "¿Está seguro que desea salir de la configuración?", "Aceptar", "Cancelar");
                if (res)
                {
                    await Shell.Current.GoToAsync("..");
                }
            }
        }




    }
}
