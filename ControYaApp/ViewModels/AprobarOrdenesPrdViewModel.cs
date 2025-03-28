using System.Collections.ObjectModel;
using System.Windows.Input;
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
            //UnpprPtNotificadosPrd = new(await GetUnapprPtNotificadosPrd());
            //UnpprMpNotificadosPrd = new(await GetUnapprMpNotificadosPrd());
        }


        private void SelectAllPt()
        {
            foreach (var notificado in UnapprPtNotificadosPrd)
            {
                notificado.IsSelected = true;
            }
        }


        private void SelectAllMp()
        {
            foreach (var notificado in UnapprMpNotificadosPrd)
            {
                notificado.IsSelected = true;
            }
        }

        private async Task ApproveSelectedAsync()
        {

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
