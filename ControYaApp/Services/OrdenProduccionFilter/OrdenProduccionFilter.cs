﻿using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Core.Extensions;
using ControYaApp.Models;

namespace ControYaApp.Services.OrdenProduccionFilter
{
    public class OrdenProduccionFilter
    {
        public enum OrdenesProduccionFilters
        {
            All,
            PendingSaldo,
            NoPendingSaldo
        }

        public ObservableCollection<OrdenProduccionGroup> FilteredOrdenesProduccionGroup(OrdenesProduccionFilters filter, List<OrdenProduccionGroup> ordenesProduccionGroups)
        {
            // TODO: Revisar la condicion para notificado y saldo, por que son opuestos.
            //       Además, agregra un if para 'All'.

            if (OrdenesProduccionFilters.NoPendingSaldo == filter)
            {
                return ordenesProduccionGroups
                    .Where(opg => opg.All(oppt => oppt.Saldo == 0)).ToObservableCollection();

            }
            if (OrdenesProduccionFilters.PendingSaldo == filter)
            {
                var a = ordenesProduccionGroups
                    .Where(opg => opg.All(oppt => oppt.Saldo > 0)).ToObservableCollection();
                return a;
            }
            return ordenesProduccionGroups.ToObservableCollection();

        }
    }
}
