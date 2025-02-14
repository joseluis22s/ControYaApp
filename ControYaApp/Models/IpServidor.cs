using ControYaApp.ViewModels;
using SQLite;

namespace ControYaApp.Models
{
    public class IpServidor : ViewModelBase
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }


        public string? Protocolo { get; set; }

        public string? Ip { get; set; }
    }
}
