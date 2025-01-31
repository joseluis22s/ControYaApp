#nullable disable
using System.Text.Json.Serialization;

namespace ControYaApp.Models
{
    public partial class Usuario
    {
        public string NombreUsuario { get; set; }

        public string Contrasena { get; set; }

        [JsonIgnore]
        public string UsuarioSistema { get; set; }

    }
}
