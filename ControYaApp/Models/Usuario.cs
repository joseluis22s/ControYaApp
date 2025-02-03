#nullable disable
using Newtonsoft.Json;

namespace ControYaApp.Models
{
    public partial class Usuario
    {
        public string NombreUsuario { get; set; }

        public string Contrasena { get; set; }

        public string UsuarioSistema { get; set; }

        [JsonIgnore]
        public string Bloqueado { get; set; }

    }
}
