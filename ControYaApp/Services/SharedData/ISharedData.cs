﻿using System.Collections.ObjectModel;
using ControYaApp.Models;

namespace ControYaApp.Services.SharedData
{
    public interface ISharedData
    {
        string? NombreUsuario { get; set; }

        string? UsuarioSistema { get; set; }

        string? IpAddress { get; set; }

        string? Protocolo { get; set; }

        bool AutoApproveProduccion { get; set; }

        bool AutoApproveInventario { get; set; }

        bool EnableLotes { get; set; }

        ObservableCollection<OrdenProduccionGroup> AllOrdenesProduccionGroups { get; set; }

    }
}
