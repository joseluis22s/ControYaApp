#nullable disable
using ControYaApp.ViewModels;

namespace ControYaApp.Models
{
    public class OrdenProduccionDetalle : ViewModelBase
    {
        private decimal? _notificado;

        public string Detalle { get; set; }

        public string CodigoMaterial { get; set; }

        public string CodigoProducto { get; set; }

        public string Producto { get; set; }

        public string CodigoUnidad { get; set; }

        public float Cantidad { get; set; }

        public decimal? Saldo => ((decimal)Cantidad) - Notificado;

        public decimal? Notificado
        {
            get => _notificado;
            set
            {
                if (SetProperty(ref _notificado, value))
                {
                    // Notifica que Saldo ha cambiado cuando Notificado se actualiza
                    OnPropertyChanged(nameof(Saldo));
                }
            } // Notifica el cambio
        }

        public OrdenProduccionCabecera Cabecera { get; set; }
    }
}
