using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Input;
using ControYaApp.Models;
using ControYaApp.Services.DI;
using ControYaApp.Services.WebService;
using ControYaApp.ViewModels.Base;

namespace ControYaApp.ViewModels
{
    public class AprobarOrdenesPrdInvViewModel : BaseViewModel
    {

        private readonly LocalRepoService _localRepoService;

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



        public AprobarOrdenesPrdInvViewModel(LocalRepoService localRepoService, RestService restService)
        {
            _localRepoService = localRepoService;
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
                //await Toast.Make("No existen registros de PT").Show();
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
                //await Toast.Make("No existen registros de MP").Show();
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
                await Toast.Make("Sin conexión. No se puede realizar esta acción", ToastDuration.Long).Show();
                return;
            }

            if ((UnapprPtNotificadosInv is not null && UnapprPtNotificadosInv.Count != 0) &&
                (UnapprMpNotificadosInv is not null && UnapprMpNotificadosInv.Count != 0))
            {
                var selectedPtNotificados = UnapprPtNotificadosInv.Where(pt => pt.IsSelected == true).ToList();
                var selectedMpNotificados = UnapprMpNotificadosInv.Where(mp => mp.IsSelected == true).ToList();
                if (selectedPtNotificados.Count == 0 && selectedMpNotificados.Count == 0)
                {
                    await Toast.Make("Ningún registro seleccionado").Show();
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
                approvedPtNotificados = approvedPtNotificados,
                approvedMpNotificados = approvedMpNotificados
            };
            try
            {
                await _restService.ApprovePtMpNotificados(req);

                // PARA QUE EL USUARIO TENGA FEEDBACK DE LO QUE OCURRE
                // - Se eliminan todos los PT y MP "sincronizados" de la DB local.
                await _localRepoService.PtNotificadoRepo.DeleteSyncApprovedPtNotificado();
                await _localRepoService.MpNotificadoRepo.DeleteSyncApprovedMpNotificado();

                // - Se consultan todos los PT y MP no aprobados pero si sincronizados a la API.
                var ptNotificados = await _restService.GetUnapproveddPtPrdInv();
                var mpNotificados = await _restService.GetUnapproveddMpPrdInv();

                // - Se guardan de nuevo a la DB
                await _localRepoService.PtNotificadoRepo.SaveAllUnapprPtNotficado(ptNotificados);
                await _localRepoService.MpNotificadoRepo.SaveAllUnapprMpNotficado(mpNotificados);

                InitData();
            }
            catch (Exception ex)
            {
                await Toast.Make(ex.Message, ToastDuration.Long).Show();
            }
        }


        private async Task<List<PtNotificado>> GetUnapprPtNotificadosInv()
        {
            return await _localRepoService.PtNotificadoRepo.GetUnapprPtNotificadosInv();
        }


        private async Task<List<MpNotificado>> GetUnapprMpNotificadosInv()
        {
            return await _localRepoService.MpNotificadoRepo.GetUnapprMpNotificadosInv();
        }
    }
}
