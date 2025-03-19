using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using ControYaApp.Services.DI;
using ControYaApp.Services.LocalDatabase.Repositories;
using ControYaApp.Services.SharedData;
using ControYaApp.Services.WebService;

namespace ControYaApp.ViewModels
{
    public partial class AppShellViewModel : BaseViewModel
    {

        private readonly RestService _restService;

        private readonly LocalRepoService _localRepoService;

        private IpServidorRepo _ipServidorRepo;



        private bool _isConected;
        public bool IsConected
        {
            get => _isConected;
            set => SetProperty(ref _isConected, value);
        }



        public ISharedData SharedData { get; set; }



        public ICommand GoToLoginCommand { get; }

        public ICommand FlyoutShellCommand { get; }





        public AppShellViewModel(IpServidorRepo ipServidorRepo, RestService restService, LocalRepoService localRepoService, ISharedData sharedData)
        {

            SharedData = sharedData; //No mover.

            _restService = restService;
            _localRepoService = localRepoService;
            _ipServidorRepo = ipServidorRepo;

            InitIpAddress();

            GoToLoginCommand = new AsyncRelayCommand(GoToLoginAsync);
            FlyoutShellCommand = new RelayCommand(FlyoutShell);

        }



        private async void InitIpAddress()
        {
            var ip = await _ipServidorRepo.GetIpServidorAsync();
            if (ip is null)
            {
                SharedData.IpAddress = "";
                SharedData.Protocolo = "http://";
            }
            else
            {
                SharedData.IpAddress = ip.Ip;
                SharedData.Protocolo = ip.Protocolo;
            }
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






    }
}
