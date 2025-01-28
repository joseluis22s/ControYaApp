#nullable disable
namespace ControYaApp.Models
{
    public class OrdenProduccion
    {

        public string Centro { get; set; }

        public string CodigoProduccion { get; set; }

        public int Orden { get; set; }

        public DateTime Fecha { get; set; }

        public string Referencia { get; set; }

        public string Detalle { get; set; }

        public string CodigoMaterial { get; set; }

        public string CodigoProducto { get; set; }

        public string Producto { get; set; }

        public string CodigoUnidad { get; set; }

        public float Cantidad { get; set; }

        public decimal? Notificado { get; set; }
    }
}
