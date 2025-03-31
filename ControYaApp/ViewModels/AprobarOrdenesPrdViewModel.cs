using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Input;
using ControYaApp.Models;
using ControYaApp.Services.DI;
using ControYaApp.Services.WebService;

namespace ControYaApp.ViewModels
{
    public class AprobarOrdenesPrdViewModel : BaseViewModel
    {
        private readonly LocalRepoService _localRepoService;

        private readonly RestService _restService;


        private ObservableCollection<PtNotificado> _unapprPtNotificadosPrd;
        public ObservableCollection<PtNotificado> UnapprPtNotificadosPrd
        {
            get => _unapprPtNotificadosPrd;
            set
            {
                _unapprPtNotificadosPrd = value;
                OnPropertyChanged();
            }
        }


        private ObservableCollection<MpNotificado> _unapprMpNotificadosPrd;
        public ObservableCollection<MpNotificado> UnapprMpNotificadosPrd
        {
            get => _unapprMpNotificadosPrd;
            set
            {
                _unapprMpNotificadosPrd = value;
                OnPropertyChanged();
            }
        }


        public ICommand SelectAllPtCommand { get; }

        public ICommand SelectAllMpCommand { get; }

        public ICommand ApproveSelectedCommand { get; }



        public AprobarOrdenesPrdViewModel(LocalRepoService localRepoService, RestService restService)
        {
            _localRepoService = localRepoService;
            _restService = restService;

            SelectAllPtCommand = new RelayCommand(SelectAllPt);
            SelectAllMpCommand = new RelayCommand(SelectAllMp);
            ApproveSelectedCommand = new AsyncRelayCommand(ApproveSelectedAsync);

            InitData();
        }

        private async void InitData()
        {
            UnapprPtNotificadosPrd = new(await GetUnapprPtNotificadosPrd());
            UnapprMpNotificadosPrd = new(await GetUnapprMpNotificadosPrd());
        }


        private void SelectAllPt()
        {
            if (UnapprPtNotificadosPrd is null || UnapprPtNotificadosPrd.Count == 0)
            {
                //await Toast.Make("No existen registros de PT").Show();
                return;
            }
            foreach (var notificado in UnapprPtNotificadosPrd)
            {
                notificado.IsSelected = true;
            }
        }


        private void SelectAllMp()
        {
            if (UnapprMpNotificadosPrd is null || UnapprMpNotificadosPrd.Count == 0)
            {
                //await Toast.Make("No existen registros de PT").Show();
                return;
            }
            foreach (var notificado in UnapprMpNotificadosPrd)
            {
                notificado.IsSelected = true;
            }
        }

        private async Task ApproveSelectedAsync()
        {
            NetworkAccess accessType = Connectivity.Current.NetworkAccess;

            if (accessType != NetworkAccess.Internet)
            {
                await Toast.Make("Sin conexión para realizar esta acción", ToastDuration.Long).Show();
                return;
            }
            List<PtNotificado> approvedPtNotificados = [];
            List<MpNotificado> approvedMpNotificados = [];
            if (UnapprPtNotificadosPrd is not null && UnapprPtNotificadosPrd.Count != 0)
            {
                approvedPtNotificados = UnapprPtNotificadosPrd.Where(pt => pt.IsSelected == true).ToList();
                foreach (var notificado in UnapprPtNotificadosPrd)
                {
                    notificado.AprobarAutoProduccion = true;
                }
            }
            if (UnapprMpNotificadosPrd is not null && UnapprMpNotificadosPrd.Count != 0)
            {
                approvedMpNotificados = UnapprMpNotificadosPrd.Where(mp => mp.IsSelected == true).ToList();
                foreach (var notificado in UnapprMpNotificadosPrd)
                {
                    notificado.AprobarAutoProduccion = true;
                }
            }
            var req = new
            {
                approvedPtNotificados = approvedPtNotificados,
                approvedMpNotificados = approvedMpNotificados
            };
            await _restService.ApprovePtMpNotificados(req);
        }


        private async Task<List<PtNotificado>> GetUnapprPtNotificadosPrd()
        {
            return await _localRepoService.PtNotificadoRepo.GetUnapprPtNotificadosPrd();
        }


        private async Task<List<MpNotificado>> GetUnapprMpNotificadosPrd()
        {
            return await _localRepoService.MpNotificadoRepo.GetUnapprMpNotificadosPrd();
        }
    }
}
