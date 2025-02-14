using ControYaApp.ViewModels;
using SQLite;

namespace ControYaApp.Models
{
    public class IpServidor : ViewModelBase
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        //private string? _protocolo;

        //private string? _ip;

        //public string? Protocolo
        //{
        //    get => _protocolo;
        //    set => SetProperty(ref _protocolo, value);
        //}

        //public string? Ip
        //{

        //    get => _ip;
        //    set => SetProperty(ref _ip, value);
        //}
        public string? Protocolo { get; set; }

        public string? Ip { get; set; }
    }
}
