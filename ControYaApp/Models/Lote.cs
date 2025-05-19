using System.Text.Json.Serialization;
using SQLite;

namespace CbMovil.Models
{
    public class Lote
    {
        [JsonIgnore]
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Nombre { get; set; }

        [JsonIgnore]
        public bool Sincronizar { get; set; }

        public bool Habilitado { get; set; } = true;
    }
}
