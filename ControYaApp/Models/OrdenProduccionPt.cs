using System.Text.Json.Serialization;
using ControYaApp.Services.NotifyPropertyChanged;
using SQLite;

namespace ControYaApp.Models
{
    public partial class OrdenProduccionPt : NotifyPropertyChanged
    {

        [JsonIgnore]
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; }

        [JsonIgnore]
        [Ignore]
        public decimal Saldo => ((decimal)Cantidad) - Notificado;

        [JsonIgnore]
        [Ignore]
        public decimal Porcentaje => (Notificado / ((decimal)Cantidad)) * 100;

        public string Centro { get; set; }

        public string CodigoProduccion { get; set; }

        public int Orden { get; set; }

        public string CodigoProducto { get; set; }

        public string CodigoMaterial { get; set; }

        public string Producto { get; set; }

        public string CodigoUnidad { get; set; }

        public float Cantidad { get; set; }


        private decimal _notificado;
        public decimal Notificado
        {
            get => _notificado;
            set
            {
                if (SetProperty(ref _notificado, value))
                {
                    OnPropertyChanged(nameof(Saldo));
                    OnPropertyChanged(nameof(Porcentaje));
                }
            }
        }

        public string CodigoUsuarioAprobar { get; set; }
    }
}
