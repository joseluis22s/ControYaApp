using SQLite;

namespace ControYaApp.Models
{
    public class EmpleadoSistema
    {
        [PrimaryKey]
        public string Empleado { get; set; }

        public bool Bloqueado { get; set; }
    }
}
