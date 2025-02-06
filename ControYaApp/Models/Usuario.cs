#nullable disable
using Newtonsoft.Json;
using SQLite;

namespace ControYaApp.Models
{
    public partial class Usuario
    {
        public string NombreUsuario { get; set; }

        public string Contrasena { get; set; }

        public string UsuarioSistema { get; set; }

        public string Bloqueado { get; set; }

        [JsonIgnore]
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }
    }
}
