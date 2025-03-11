using System.Text.Json.Serialization;
using SQLite;

namespace ControYaApp.Models
{
    public class OrdenProduccionPt
    {
        public string Centro { get; set; }

        public string CodigoProduccion { get; set; }

        public int Orden { get; set; }

        public string CodigoProducto { get; set; }

        public string Producto { get; set; }

        public string CodigoUnidad { get; set; }

        public float Cantidad { get; set; }

        public decimal? Notificado { get; set; }

        public string CodigoUsuario { get; set; }

        [JsonIgnore]
        [Ignore]
        public decimal? Saldo => ((decimal)Cantidad) - Notificado;

        [JsonIgnore]
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }
    }
}
