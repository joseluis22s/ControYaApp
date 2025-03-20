using System.Text.Json.Serialization;
using SQLite;

namespace ControYaApp.Models
{
    public class PmNotificado
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [JsonIgnore]
        public bool Sincronizado { get; set; }

        [JsonIgnore]
        public bool NotificacionAutorizada { get; set; }

        public DateTime Fecha { get; set; }

        public decimal Notificado { get; set; }

        public string CodigoEmpleado { get; set; }

        public string CodigoUsuario { get; set; }
    }
}
