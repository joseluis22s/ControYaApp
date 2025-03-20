using System.Windows.Input;
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


        private string _mpNotificadoCount;
        public string MpNotificadoCount
        {
            get { return _mpNotificadoCount; }
            set { _mpNotificadoCount = value; }
        }


        public ICommand AuthorizeAllCommand { get; }



        public AutorizarOrdenesPrdViewModel(LocalRepoService localRepoService)
        {
            _localRepoService = localRepoService;
        }


        //private async Task AuthorizeAllAsync()
        //{
        //    await _localRepoService.MpNotificadoRepo
        //}



    }
}
