﻿using System.Collections.ObjectModel;
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
    public partial class AprobarOrdenesPrdViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly PrdDbReposService _prdDbReposService;




        public ISharedData SharedData { get; set; }

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



        public AprobarOrdenesPrdViewModel(INavigationService navigationServie, IDialogService dialogService, ISharedData sharedData,
            PrdDbReposService prdDbReposService, RestService restService) : base(navigationServie)
        {
            _dialogService = dialogService;
            _prdDbReposService = prdDbReposService;

            SharedData = sharedData;


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
                await _dialogService.ShowToastAsync("Sin conexión. No se puede realizar esta acción", ToastDuration.Long);
                return;
            }


            if ((UnapprPtNotificadosPrd is not null && UnapprPtNotificadosPrd.Count != 0) &&
                (UnapprMpNotificadosPrd is not null && UnapprMpNotificadosPrd.Count != 0))
            {
                var selectedPtNotificados = UnapprPtNotificadosPrd.Where(pt => pt.IsSelected == true).ToList();
                var selectedMpNotificados = UnapprMpNotificadosPrd.Where(mp => mp.IsSelected == true).ToList();
                if (selectedPtNotificados.Count == 0 && selectedMpNotificados.Count == 0)
                {
                    await _dialogService.ShowToastAsync("Ningún registro seleccionado");
                    return;
                }
            }

            List<PtNotificado> approvedPtNotificados = [];
            List<MpNotificado> approvedMpNotificados = [];

            if (UnapprPtNotificadosPrd is not null && UnapprPtNotificadosPrd.Count != 0)
            {
                approvedPtNotificados = UnapprPtNotificadosPrd.Where(pt => pt.IsSelected == true).ToList();
                foreach (var ptNotificado in approvedPtNotificados)
                {
                    ptNotificado.AprobarAutoProduccion = true;
                }
            }
            if (UnapprMpNotificadosPrd is not null && UnapprMpNotificadosPrd.Count != 0)
            {
                approvedMpNotificados = UnapprMpNotificadosPrd.Where(mp => mp.IsSelected == true).ToList();

                foreach (var mpNotificado in UnapprMpNotificadosPrd)
                {
                    mpNotificado.AprobarAutoProduccion = true;
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
                    await _dialogService.ShowToastAsync("Error al Aprobar los PT y MP notificados", ToastDuration.Long);
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
                await _dialogService.ShowToastAsync(ex.Message, ToastDuration.Long);
            }

        }


        private async Task<List<PtNotificado>> GetUnapprPtNotificadosPrd()
        {
            return await _prdDbReposService.PtNotificadoRepo.GetUnapprPtNotificadosPrd();
        }


        private async Task<List<MpNotificado>> GetUnapprMpNotificadosPrd()
        {
            return await _prdDbReposService.MpNotificadoRepo.GetUnapprMpNotificadosPrd();
        }
    }
}
