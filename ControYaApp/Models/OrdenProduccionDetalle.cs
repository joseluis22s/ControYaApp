#nullable disable
namespace ControYaApp.Models
{
    public class OrdenProduccionDetalle
    {
        public string Detalle { get; set; }

        public string CodigoMaterial { get; set; }

        public string CodigoProducto { get; set; }

        public string Producto { get; set; }

        public string CodigoUnidad { get; set; }

        public float Cantidad { get; set; }

        public decimal? Notificado { get; set; }

        public decimal? Saldo => ((decimal)Cantidad) - Notificado;

        public OrdenProduccionCabecera Cabecera { get; set; }
    }
}
