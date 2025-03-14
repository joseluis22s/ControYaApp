using CommunityToolkit.Maui.Core;
using ControYaApp.Services.LocalDatabase.Repositories;
using ControYaApp.Services.SharedData;

namespace ControYaApp.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private readonly OrdenProduccionRepo _ordenProduccionRepo;
        private readonly OrdenProduccionPtRepo _ordenProduccionPtRepo;

        public ISharedData SharedData { get; set; }




        public HomeViewModel(ISharedData sharedData, OrdenProduccionRepo ordenProduccionRepo, OrdenProduccionPtRepo ordenProduccionPtRepo)
        {
            SharedData = sharedData;

            _ordenProduccionRepo = ordenProduccionRepo;
            _ordenProduccionPtRepo = ordenProduccionPtRepo;
        }




        internal async Task BackButtonPressed()
        {
            var res = await Shell.Current.DisplayAlert("Salir", "¿Desea cerrar sesión?", "Aceptar", "Cancelar");
            if (res)
            {
                await Shell.Current.GoToAsync("//login");
            }
        }

        public async Task ObtenerOrdenesAsync()
        {
            try
            {
                var ordenesProduccionDb = await _ordenProduccionRepo.GetOrdenesProduccionByUsuarioSistema(SharedData.UsuarioSistema);

                if (ordenesProduccionDb.Count != 0)
                {
                    var ordenesProduccionPt = await _ordenProduccionPtRepo.GetAllOrdenesProduccionPt();
                    _allOrdenesGrouped = MapOrdenesProduccionGrouped(ordenesProduccionDb, ordenesProduccionPt);
                    OrdenesProduccionGroups = FilteredOrdenesProduccionGroup(OrdenesProduccionFilters.Pending, _allOrdenesGrouped);
                    OrdenesGroupLoaded = true;
                }
                else
                {
                    await Toast.Make("No se han encontrado datos", ToastDuration.Long).Show();
                }
            }
            catch (Exception ex)
            {
                await Toast.Make(ex.Message, ToastDuration.Long).Show();
            }
            finally
            {
                await loadingPopUpp.CloseAsync();
            }

        }



    }
}
