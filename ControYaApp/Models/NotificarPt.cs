namespace ControYaApp.Models
{
    public class NotificarPt
    {
        public string CodigoProduccion { get; set; }

        public int Orden { get; set; }

        public string CodigoMaterial { get; set; }

        public string CodigoBodega { get; set; }

        public string CodigoProducto { get; set; }

        public string Producto { get; set; }

        public float Cantidad { get; set; }

        public decimal? Notificado { get; set; }
    }
}
