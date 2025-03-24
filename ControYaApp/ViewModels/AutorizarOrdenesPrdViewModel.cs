using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using ControYaApp.Services.DI;

namespace ControYaApp.ViewModels
{
    public class AutorizarOrdenesPrdViewModel
    {
        private readonly LocalRepoService _localRepoService;


        private int _ptNotificadoCount;
        public int PtNotificadoCount
        {
            get { return _ptNotificadoCount; }
            set { _ptNotificadoCount = value; }
        }


        private int _mpNotificadoCount;
        public int MpNotificadoCount
        {
            get { return _mpNotificadoCount; }
            set { _mpNotificadoCount = value; }
        }


        public ICommand AuthorizeAllCommand { get; }



        public AutorizarOrdenesPrdViewModel(LocalRepoService localRepoService)
        {
            _localRepoService = localRepoService;
        }

        private async void InitData()
        {
            try
            {
                PtNotificadoCount = 0;
                MpNotificadoCount = 0;
            }
            catch (Exception ex)
            {
                await Toast.Make(ex.Message, ToastDuration.Long).Show();
            }
        }


        private async Task AuthorizeAllAsync()
        {
            //await _localRepoService.MpNotificadoRepo
        }




    }
}
