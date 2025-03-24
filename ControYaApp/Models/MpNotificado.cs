using System.Text.Json.Serialization;
using SQLite;

namespace ControYaApp.Models
{
    public class MpNotificado
    {
        [JsonIgnore]
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [JsonIgnore]
        public bool Sincronizado { get; set; }

        public string CodigoProduccion { get; set; }

        public int Orden { get; set; }

        public string CodigoMaterial { get; set; }

        public int IdMaterialProduccion { get; set; }

        public decimal Notificado { get; set; }

        public DateTime Fecha { get; set; }

        public string CodigoUsuario { get; set; }

        public string CodigoEmpleado { get; set; }

        public bool AprobarAutoProduccion { get; set; }

        public bool AprobarAutoInventario { get; set; }
    }
}
