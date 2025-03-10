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




        internal async Task BackButtonPressed()
        {
            var res = await Shell.Current.DisplayAlert("Salir", "¿Desea cerrar sesión?", "Aceptar", "Cancelar");
            if (res)
            {
                await Shell.Current.GoToAsync("//login");
            }
        }




    }
}
