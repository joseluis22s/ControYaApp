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



        private IpServidor _ipServidor;
        public IpServidor IpServidor
        {
            get => _ipServidor;
            set => SetProperty(ref _ipServidor, value);
        }



        public ISharedData SharedData { get; set; }



        public ICommand SaveIpServidorCommand { get; }

        public ICommand BackButtonPressedCommand { get; }




        public ConfigViewModel(IpServidorRepo ipServidorRepo, ISharedData sharedData)
        {

            SharedData = sharedData; //No mover.

            _ipServidorRepo = ipServidorRepo;

            InitIpServidor();

            SaveIpServidorCommand = new AsyncRelayCommand(SaveIpServidorAsync);
            BackButtonPressedCommand = new AsyncRelayCommand(BackButtonPressed);

        }




        private void InitIpServidor()
        {
            IpServidor.Ip = _ip = SharedData.IpAddress;
            IpServidor.Protocolo = SharedData.Protocolo;
        }


        private async Task SaveIpServidorAsync()
        {

            try
            {
                string ip = IpServidor.Protocolo + IpServidor.Ip;

                var res = await Shell.Current.DisplayAlert("Nueva conexión", $"¿Está seguro que desea guardar la siguiente ip?:\n{ip}", "Aceptar", "Cancelar");

                if (res)
                {
                    await _ipServidorRepo.SaveIpServidor(IpServidor);
                    _isSaved = true;
                }

                await Shell.Current.GoToAsync("..");

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

            if (string.IsNullOrEmpty(IpServidor.Ip))
            {
                var res = await Shell.Current.DisplayAlert("Alerta", "¿Está seguro que desea salir sin guardar una dirección IP?", "Aceptar", "Cancelar");
                if (res)
                {
                    await Shell.Current.GoToAsync("..");
                    return;
                }
            }

            if (_ip != IpServidor.Ip)
            {
                var res = await Shell.Current.DisplayAlert("Alerta", "¿Está seguro que desea salir sin guardar los cambios?", "Aceptar", "Cancelar");
                if (res)
                {
                    await Shell.Current.GoToAsync("..");
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
