#nullable disable
using System.Text.Json.Serialization;
using ControYaApp.ViewModels;
using SQLite;

namespace ControYaApp.Models
{
    public class OrdenProduccion : BaseViewModel
    {

        private DateTime _fecha;

        public string Centro { get; set; }

        public string CodigoProduccion { get; set; }

        public int Orden { get; set; }

        [JsonPropertyName("codigoUsuarioAprobar")]
        public string CodigoUsuario { get; set; }

        public DateTime Fecha
        {
            get => _fecha;
            set => SetProperty(ref _fecha, value);
        }

        public string Referencia { get; set; }

        public string Detalle { get; set; }

        public string CodigoMaterial { get; set; }

        public string CodigoProducto { get; set; }

        public string Producto { get; set; }

        public string CodigoUnidad { get; set; }

        public float Cantidad { get; set; }

        public decimal? Notificado { get; set; }

        [JsonIgnore]
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }
    }
}
