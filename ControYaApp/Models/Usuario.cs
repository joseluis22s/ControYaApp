#nullable disable
using ControYaApp.ViewModels;
using Newtonsoft.Json;
using SQLite;

namespace ControYaApp.Models
{
    public partial class Usuario : BaseViewModel
    {
        public string NombreUsuario { get; set; }

        public string Contrasena { get; set; }

        public string UsuarioSistema { get; set; }

        [JsonIgnore]
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }
    }
}
