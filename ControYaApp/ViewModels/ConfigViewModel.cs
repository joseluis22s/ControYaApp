using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.Input;
using ControYaApp.Models;
using ControYaApp.Services.LocalDatabase.Repositories;

namespace ControYaApp.ViewModels
{
    public class ConfigViewModel : ViewModelBase
    {
        private bool _isSaved;

        private readonly IpServidorRepo _ipServidorRepo;


        private IpServidor _ipServidor;

        public IpServidor IpServidor
        {
            get => _ipServidor;
            set => SetProperty(ref _ipServidor, value);
        }



        public ICommand SaveIpServidorCommand { get; }

        public ICommand BackButtonPressedCommand { get; }



        public ConfigViewModel(IpServidorRepo ipServidorRepo)
        {
            SaveIpServidorCommand = new AsyncRelayCommand(SaveIpServidorAsync);
            BackButtonPressedCommand = new AsyncRelayCommand(BackButtonPressed);

            _ipServidorRepo = ipServidorRepo;

            InitializeIpServidor();
        }

        private async void InitializeIpServidor()
        {
            IpServidor = await GetIpServidorAsync();
        }

        private async Task<IpServidor> GetIpServidorAsync()
        {
            try
            {
                var ip = await _ipServidorRepo.GetIpServidorAsync();
                if (ip is not null)
                {
                    return ip;
                }
            }
            catch (Exception ex)
            {
                await Toast.Make(ex.Message).Show();
            }
            return new IpServidor { Protocolo = "http://", Ip = "" };
        }

        private async Task SaveIpServidorAsync()
        {

            try
            {
                if (string.IsNullOrWhiteSpace(IpServidor.Ip))
                {
                    await Toast.Make("El campo 'IP' no debe estar vacío").Show();
                    return;
                }

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

        public async Task BackButtonPressed()
        {
            if (_isSaved)
            {
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                var res = await Shell.Current.DisplayAlert("Alerta", "¿Está seguro que desea salir sin guardar una dirección IP?", "Aceptar", "Cancelar");
                if (res)
                {
                    await Shell.Current.GoToAsync("..");
                }
            }

        }
    }
}
