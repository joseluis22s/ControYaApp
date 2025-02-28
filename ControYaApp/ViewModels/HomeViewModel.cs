using ControYaApp.Services.SharedData;

namespace ControYaApp.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {

        public ISharedData SharedData { get; set; }




        public HomeViewModel(ISharedData sharedData)
        {
            SharedData = sharedData;
        }





    }
}
