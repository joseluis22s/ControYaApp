using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Core.Extensions;
using ControYaApp.Models;

namespace ControYaApp.Services.OrdenProduccionFilter
{
    public class OrdenProduccionFilter
    {
        public enum OrdenesProduccionFilters
        {
            All,
            Pending,
            Notified
        }

        public ObservableCollection<OrdenProduccionGroup> FilteredOrdenesProduccionGroup(OrdenesProduccionFilters filter, ObservableCollection<OrdenProduccionGroup> ordenesProduccionGroups)
        {
            // TODO: Revisar la condicion para notificado y saldo, por que son opuestos.
            //       Además, agregra un if para 'All'.

            if (OrdenesProduccionFilters.Notified == filter)
            {
                return ordenesProduccionGroups
                    .Where(opg => opg.All(oppt => oppt.Notificado <= 0)).ToObservableCollection();

            }
            if (OrdenesProduccionFilters.Pending == filter)
            {
                return ordenesProduccionGroups
                    .Where(opg => opg.All(oppt => oppt.Saldo > 0)).ToObservableCollection(); ;
            }

            return ordenesProduccionGroups;
        }
    }
}
