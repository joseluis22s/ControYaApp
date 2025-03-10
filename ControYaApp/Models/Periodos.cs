using System.Text.Json.Serialization;
using SQLite;

namespace ControYaApp.Models
{
    public class Periodos
    {

        public DateTime FechaMin { get; set; }

        public DateTime FechaMax { get; set; }

        [JsonIgnore]
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }
    }
}
