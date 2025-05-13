using SQLite;

namespace ControYaApp.Models
{
    public class DataConfig
    {
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }

        public string? Protocolo { get; set; }

        public string? Ip { get; set; }

        public bool AutoApproveProduccion { get; set; }

        public bool AutoApproveInventario { get; set; }

        public bool EnableSeries { get; set; }
    }
}
