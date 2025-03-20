using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Core.Extensions;
using ControYaApp.Models;

namespace ControYaApp.Services.OrdenProduccionFilter
{
    public class OrdenProduccionMpFilter
    {
        public enum OrdenesProduccionMpFilters
        {
            All,
            Pending
        }

        public ObservableCollection<OrdenProduccionMaterialGroup> FilteredOrdenesProduccionMpGroup(OrdenesProduccionMpFilters filter,
                                                                        ObservableCollection<OrdenProduccionMaterialGroup> ordenesProduccionMaterialGroups)
        {
            // TODO: Revisar la condicion para notificado y saldo, por que son opuestos.
            //       Además, agregra un if para 'All'.

            if (OrdenesProduccionMpFilters.Pending == filter)
            {
                return ordenesProduccionMaterialGroups
                    .Where(opg => opg.All(oppt => oppt.Saldo == 0)).ToObservableCollection();

            }

            return ordenesProduccionMaterialGroups;
        }
    }
}
