﻿using SQLite;

namespace ControYaApp.Models
{
    public class EmpleadoSistema
    {
        [PrimaryKey]
        public string CodigoEmpleado { get; set; }

        public string NombreEmpleado { get; set; }
    }
}
