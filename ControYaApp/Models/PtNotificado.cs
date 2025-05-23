﻿using System.Text.Json.Serialization;
using ControYaApp.Services.NotifyPropertyChanged;
using SQLite;

namespace ControYaApp.Models
{
    public partial class PtNotificado : MauiNotifyPropertyChanged
    {
        [JsonIgnore]
        [PrimaryKey, AutoIncrement]
        public int IdPtNotificado { get; set; }

        private bool _isSelected;
        [Ignore]
        [JsonIgnore]
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public int Id { get; set; }

        public string CodigoProduccion { get; set; }

        public int Orden { get; set; }

        public string CodigoMaterial { get; set; }

        public string Producto { get; set; }

        public DateTime Fecha { get; set; }

        public decimal Notificado { get; set; }

        public string CodigoUsuario { get; set; }

        public string CodigoEmpleado { get; set; }

        // TODO: Verificar si entra en conflicto con la deserializacion
        public bool AprobarAutoProduccion { get; set; }

        public bool AprobarAutoInventario { get; set; }

        public string SerieLote { get; set; }

        public bool Sincronizado { get; set; }

        public string Detalles { get; set; }
    }
}
