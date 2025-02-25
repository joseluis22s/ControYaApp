using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.Input;
using ControYaApp.Models;
using ControYaApp.Services.LocalDatabase.Repositories;

namespace ControYaApp.ViewModels
{
    public class ConfigViewModel : ViewModelBase
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

            _ip = IpServidor.Ip;
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
                string? ip = IpServidor.Ip;
                if (string.IsNullOrEmpty(ip))
                {
                    var res = await Shell.Current.DisplayAlert("Alerta", "¿Está seguro que desea salir sin guardar una dirección IP?", "Aceptar", "Cancelar");
                    if (res)
                    {
                        await Shell.Current.GoToAsync("..");
                    }
                }
                else if (_ip != ip)
                {
                    var res = await Shell.Current.DisplayAlert("Alerta", "¿Está seguro que desea salir sin guardar los cambios?", "Aceptar", "Cancelar");
                    if (res)
                    {
                        await Shell.Current.GoToAsync("..");
                    }
                }
                else
                {
                    var res = await Shell.Current.DisplayAlert("Alerta", "¿Está seguro que desea salir?", "Aceptar", "Cancelar");
                    if (res)
                    {
                        await Shell.Current.GoToAsync("..");
                    }
                }
            }

        }
    }
}
