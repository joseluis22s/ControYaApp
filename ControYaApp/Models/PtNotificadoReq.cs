using System.Text.Json.Serialization;
using SQLite;

namespace ControYaApp.Models
{
    public class PtNotificadoReq
    {
        public string CodigoProduccion { get; set; }
        public int Orden { get; set; }
        public string CodigoMaterial { get; set; }
        public DateTime Fecha { get; set; }
        public decimal? Notificado { get; set; }
        public string CodigoEmpleado { get; set; }
        public string Serie { get; set; }
        public string Usuario { get; set; }

        [JsonIgnore]
        public bool Sincronizado { get; set; }

        [JsonIgnore]
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }
    }
}
