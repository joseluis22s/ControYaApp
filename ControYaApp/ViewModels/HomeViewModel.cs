using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using ControYaApp.Models;
using ControYaApp.Services.LocalDatabase.Repositories;
using ControYaApp.Services.OrdenProduccionFilter;
using ControYaApp.Services.SharedData;

namespace ControYaApp.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private readonly OrdenProduccionRepo _ordenProduccionRepo;
        private readonly OrdenProduccionPtRepo _ordenProduccionPtRepo;
        private OrdenProduccionFilter OrdenProduccionFilter { get; set; }

        public ISharedData SharedData { get; set; }




        public HomeViewModel(ISharedData sharedData, OrdenProduccionRepo ordenProduccionRepo, OrdenProduccionPtRepo ordenProduccionPtRepo)
        {
            SharedData = sharedData;

            _ordenProduccionRepo = ordenProduccionRepo;
            _ordenProduccionPtRepo = ordenProduccionPtRepo;

            InitData();
        }

        private async void InitData()
        {
            SharedData.AllOrdenesProduccionGroups = await GetAllOrdenesProduccionAsync();
        }


        internal async Task BackButtonPressed()
        {
            var res = await Shell.Current.DisplayAlert("Salir", "¿Desea cerrar sesión?", "Aceptar", "Cancelar");
            if (res)
            {
                await Shell.Current.GoToAsync("//login");
            }
        }

        public async Task<ObservableCollection<OrdenProduccionGroup>> GetAllOrdenesProduccionAsync()
        {
            try
            {
                var ordenesProduccionDb = await _ordenProduccionRepo.GetOrdenesProduccionByUsuarioSistema(SharedData.UsuarioSistema);

                if (ordenesProduccionDb.Count == 0)
                {
                    await Toast.Make("No se han encontrado ordenes de producción", ToastDuration.Long).Show();
                    return [];
                }
                else
                {
                    var ordenesProduccionPt = await _ordenProduccionPtRepo.GetAllOrdenesProduccionPt();
                    return MapOrdenesProduccionGrouped(ordenesProduccionDb, ordenesProduccionPt);
                    //var a = FilteredOrdenesProduccionGroup(OrdenProduccionFilter.OrdenesProduccionFilters.Pending, allOrdenesProduccionGroups);
                }
            }
            catch (Exception ex)
            {
                await Toast.Make(ex.Message, ToastDuration.Long).Show();
            }
            return [];
        }

        private ObservableCollection<OrdenProduccionGroup> MapOrdenesProduccionGrouped(ObservableCollection<OrdenProduccion> ordenesPrd, ObservableCollection<OrdenProduccionPt> ordenesProducciondPt)
        {
            var ordenProducciondPtDic = ordenesProducciondPt
                .GroupBy(d => new
                {
                    d.Centro,
                    d.CodigoProduccion,
                    d.Orden
                }
                ).ToDictionary(g => g.Key, g => g.ToList());


            var ordenesProduccionGrouped = ordenesPrd
                .Select(ordenProduccion =>
                {
                    var key = new
                    {
                        ordenProduccion.Centro,
                        ordenProduccion.CodigoProduccion,
                        ordenProduccion.Orden
                    };

                    return new OrdenProduccionGroup(
                        ordenProduccion,
                        ordenProducciondPtDic.TryGetValue(key, out var ordenesProducciondPtGrouped) ? ordenesProducciondPtGrouped : new List<OrdenProduccionPt>()
                    );
                })
                .ToList();

            return new ObservableCollection<OrdenProduccionGroup>(ordenesProduccionGrouped);
        }

        public ObservableCollection<OrdenProduccionGroup> FilteredOrdenesProduccionGroup(OrdenProduccionFilter.OrdenesProduccionFilters filter, ObservableCollection<OrdenProduccionGroup> ordenesProduccionGroups)
        {
            if (OrdenProduccionFilter.OrdenesProduccionFilters.Notified == filter)
            {
                return ordenesProduccionGroups
                    .Where(opg => opg.All(oppt => oppt.Notificado == 0)).ToObservableCollection();

            }
            if (OrdenProduccionFilter.OrdenesProduccionFilters.Pending == filter)
            {
                return ordenesProduccionGroups
                    .Where(opg => opg.All(oppt => oppt.Saldo > 0)).ToObservableCollection(); ;
            }

            return ordenesProduccionGroups;
        }


    }
}
