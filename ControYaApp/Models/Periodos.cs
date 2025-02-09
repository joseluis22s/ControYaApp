﻿using Newtonsoft.Json;
using SQLite;

namespace ControYaApp.Models
{
    public class Periodos
    {
        public int Ejercicio { get; set; }

        public int Periodo { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        public bool Estado { get; set; }

        [JsonIgnore]
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }
    }
}
