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

        // TODO: Verificar si entra en conflicto con la deserializacion
        public bool AprobarAutoProduccion { get; set; }

        public bool AprobarAutoInventario { get; set; }

        public DateTime Fecha { get; set; }

        public decimal Notificado { get; set; }

        public string CodigoEmpleado { get; set; }

        public string CodigoUsuario { get; set; }
    }
}
