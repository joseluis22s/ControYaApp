using SQLite;

namespace ControYaApp.Models
{
    public class IpServidor
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Ip { get; set; }
    }
}
