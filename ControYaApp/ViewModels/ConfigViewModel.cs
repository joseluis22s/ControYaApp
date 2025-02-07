using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.Input;
using ControYaApp.Models;
using ControYaApp.Services.LocalDatabase.Repositories;

namespace ControYaApp.ViewModels
{
    public class ConfigViewModel : ViewModelBase
    {
        private readonly IpServidorRepo _ipServidorRepo;



        private string? _ipServidor;



        public string? IpServidor
        {
            get => _ipServidor;
            set => SetProperty(ref _ipServidor, value);
        }



        public ICommand SaveIpServidorCommand { get; }



        public ConfigViewModel(IpServidorRepo ipServidorRepo)
        {


            SaveIpServidorCommand = new AsyncRelayCommand(SaveIpServidorAsync);

            _ipServidorRepo = ipServidorRepo;

            InitializeIpServidor();
        }

        private async void InitializeIpServidor()
        {
            IpServidor = await GetIpServidorAsync();
        }

        private async Task<string> GetIpServidorAsync()
        {
            try
            {
                var ip = await _ipServidorRepo.GetIpServidorAsync();
                if (!string.IsNullOrEmpty(ip))
                {
                    return ip;
                }
            }
            catch (Exception ex)
            {
                await Toast.Make(ex.Message).Show();
            }
            return "";
        }

        private async Task SaveIpServidorAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(IpServidor))
                {
                    await Toast.Make("El campo no debe estar vacío").Show();
                    return;
                }

                var res = await Shell.Current.DisplayAlert("Nueva conexión", "¿Está seguro que desea guardar una nueva dirección IP?", "Aceptar", "Cancelar");

                if (res)
                {
                    IpServidor ipServidor = new IpServidor { Ip = IpServidor };
                    await _ipServidorRepo.SaveIpServidor(ipServidor);
                }

                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Toast.Make(ex.Message).Show();
            }
        }


    }
}
