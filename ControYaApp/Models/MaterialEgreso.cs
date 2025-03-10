using System.Text.Json.Serialization;
using SQLite;

namespace ControYaApp.Models
{
    public class MaterialEgreso
    {
        public string CodigoProduccion { get; set; }

        public int Orden { get; set; }

        public string CodigoMaterial { get; set; }

        public string Material { get; set; }

        public string CodigoBodega { get; set; }

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
