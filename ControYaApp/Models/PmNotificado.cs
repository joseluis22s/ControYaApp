using System.Text.Json.Serialization;
using SQLite;

namespace ControYaApp.Models
{
    public class PmNotificado
    {
        [JsonIgnore]
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }

        [JsonIgnore]
        public bool Sincronizado { get; set; }

        [JsonIgnore]
        public bool Autorizado { get; set; }

        public string CodigoProduccion { get; set; }

        public int Orden { get; set; }

        public string CodigoMaterial { get; set; }

        public string CodigoBodega { get; set; }

        public string CodigoProducto { get; set; }

        public string CodigoUnidad { get; set; }

        public decimal Cantidad { get; set; }

        public decimal Notificado { get; set; }

        public decimal Saldo { get; set; }
    }
}
