using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Input;
using ControYaApp.Models;
using ControYaApp.Services.Dialog;
using ControYaApp.Services.LocalDatabase;
using ControYaApp.Services.Navigation;
using ControYaApp.Services.SharedData;
using ControYaApp.Services.WebService;
using ControYaApp.ViewModels.Base;

namespace ControYaApp.ViewModels
{
    public partial class AprobarOrdenesPrdInvViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly PrdDbReposService _prdDbReposService;

        public ISharedData SharedData { get; set; }


        private readonly RestService _restService;


        private bool _isEnableApproveButton;
        public bool IsEnableApproveButton
        {
            get => _isEnableApproveButton;
            set => SetProperty(ref _isEnableApproveButton, value);
        }


        private ObservableCollection<PtNotificado> _unapprPtNotificadosInv;
        public ObservableCollection<PtNotificado> UnapprPtNotificadosInv
        {
            get => _unapprPtNotificadosInv;
            set => SetProperty(ref _unapprPtNotificadosInv, value);
        }


        private ObservableCollection<MpNotificado> _unapprMpNotificadosInv;
        public ObservableCollection<MpNotificado> UnapprMpNotificadosInv
        {
            get => _unapprMpNotificadosInv;
            set => SetProperty(ref _unapprMpNotificadosInv, value);
        }


        public ICommand SelectAllPtCommand { get; }

        public ICommand SelectAllMpCommand { get; }

        public ICommand ApproveSelectedCommand { get; }



        public AprobarOrdenesPrdInvViewModel(INavigationService navigationServie, IDialogService dialogService, ISharedData sharedData,
            PrdDbReposService prdDbReposService, RestService restService) : base(navigationServie)
        {
            _dialogService = dialogService;
            _prdDbReposService = prdDbReposService;


            SharedData = sharedData;

            _restService = restService;

            SelectAllPtCommand = new RelayCommand(SelectAllPt);
            SelectAllMpCommand = new RelayCommand(SelectAllMp);
            ApproveSelectedCommand = new RelayCommand(ApproveSelectedAsync);

            InitData();
        }

        private async void InitData()
        {
            UnapprPtNotificadosInv = new(await GetUnapprPtNotificadosInv());
            UnapprMpNotificadosInv = new(await GetUnapprMpNotificadosInv());
        }


        private void SelectAllPt()
        {
            if (UnapprPtNotificadosInv is null || UnapprPtNotificadosInv.Count == 0)
            {
                return;
            }
            foreach (var notificado in UnapprPtNotificadosInv)
            {
                notificado.IsSelected = true;
            }
        }


        private void SelectAllMp()
        {
            if (UnapprMpNotificadosInv is null || UnapprMpNotificadosInv.Count == 0)
            {
                return;
            }
            foreach (var notificado in UnapprMpNotificadosInv)
            {
                notificado.IsSelected = true;
            }
        }

        private async void ApproveSelectedAsync()
        {
            NetworkAccess accessType = Connectivity.Current.NetworkAccess;

            if (accessType != NetworkAccess.Internet)
            {
                await _dialogService.ShowToast("Sin conexión. No se puede realizar esta acción", ToastDuration.Long);
                return;
            }

            if ((UnapprPtNotificadosInv is not null && UnapprPtNotificadosInv.Count != 0) &&
                (UnapprMpNotificadosInv is not null && UnapprMpNotificadosInv.Count != 0))
            {
                var selectedPtNotificados = UnapprPtNotificadosInv.Where(pt => pt.IsSelected == true).ToList();
                var selectedMpNotificados = UnapprMpNotificadosInv.Where(mp => mp.IsSelected == true).ToList();
                if (selectedPtNotificados.Count == 0 && selectedMpNotificados.Count == 0)
                {
                    await _dialogService.ShowToast("Ningún registro seleccionado");
                    return;
                }
            }

            List<PtNotificado> approvedPtNotificados = [];
            List<MpNotificado> approvedMpNotificados = [];

            if (UnapprPtNotificadosInv is not null && UnapprPtNotificadosInv.Count != 0)
            {
                approvedPtNotificados = UnapprPtNotificadosInv.Where(pt => pt.IsSelected == true).ToList();
                foreach (var notificado in UnapprPtNotificadosInv)
                {
                    notificado.AprobarAutoInventario = true;
                }
            }
            if (UnapprMpNotificadosInv is not null && UnapprMpNotificadosInv.Count != 0)
            {
                approvedMpNotificados = UnapprMpNotificadosInv.Where(mp => mp.IsSelected == true).ToList();
                foreach (var notificado in UnapprMpNotificadosInv)
                {
                    notificado.AprobarAutoInventario = true;
                }
            }
            var req = new
            {
                PtNotificados = approvedPtNotificados,
                MpNotificados = approvedMpNotificados
            };
            try
            {
                if (!await _restService.ProcessPtMpNotificados(req))
                {
                    await _dialogService.ShowToast("Error al Aprobar los PT y MP notificados", ToastDuration.Long);
                    return;
                }

                // PARA QUE EL USUARIO TENGA FEEDBACK DE LO QUE OCURRE
                // - Se eliminan todos los PT y MP "sincronizados" de la DB local.
                await _prdDbReposService.PtNotificadoRepo.DeleteSyncApprovedPtNotificado();
                await _prdDbReposService.MpNotificadoRepo.DeleteSyncApprovedMpNotificado();

                // - Se consultan todos los PT y MP no aprobados pero si sincronizados a la API.
                var ptNotificados = await _restService.GetUnapproveddPtPrdInv(SharedData.UsuarioSistema);
                var mpNotificados = await _restService.GetUnapproveddMpPrdInv(SharedData.UsuarioSistema);

                // - Se guardan de nuevo a la DB
                await _prdDbReposService.PtNotificadoRepo.SaveAllUnapprPtNotficado(ptNotificados);
                await _prdDbReposService.MpNotificadoRepo.SaveAllUnapprMpNotficado(mpNotificados);

                InitData();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowToast(ex.Message, ToastDuration.Long);
            }
        }


        private async Task<List<PtNotificado>> GetUnapprPtNotificadosInv()
        {
            return await _prdDbReposService.PtNotificadoRepo.GetUnapprPtNotificadosInv();
        }


        private async Task<List<MpNotificado>> GetUnapprMpNotificadosInv()
        {
            return await _prdDbReposService.MpNotificadoRepo.GetUnapprMpNotificadosInv();
        }
    }
}
