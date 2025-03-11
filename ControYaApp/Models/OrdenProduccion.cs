#nullable disable
using System.Text.Json.Serialization;
using SQLite;

namespace ControYaApp.Models
{
    public class OrdenProduccion
    {

        public string Centro { get; set; }

        public string CodigoProduccion { get; set; }

        public int Orden { get; set; }

        public DateTime Fecha { get; set; }

        public string Referencia { get; set; }

        public string Detalle { get; set; }

        public string CodigoUsuarioAprobar { get; set; }

        [JsonIgnore]
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }
    }
}
