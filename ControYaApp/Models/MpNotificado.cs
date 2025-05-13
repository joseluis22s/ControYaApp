using System.Text.Json.Serialization;
using ControYaApp.Services.NotifyPropertyChanged;
using SQLite;

namespace ControYaApp.Models
{
    public partial class MpNotificado : MauiNotifyPropertyChanged
    {
        [JsonIgnore]
        [PrimaryKey, AutoIncrement]
        public int IdMpNotificado { get; set; }

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

        public int IdMaterialProduccion { get; set; }

        public decimal Notificado { get; set; }

        public DateTime Fecha { get; set; }

        public string CodigoUsuario { get; set; }

        public string CodigoEmpleado { get; set; }

        public bool AprobarAutoProduccion { get; set; }

        public bool AprobarAutoInventario { get; set; }

        public bool Sincronizado { get; set; }

        public string SerieLote { get; set; }

        public string Detalles { get; set; }
    }
}
