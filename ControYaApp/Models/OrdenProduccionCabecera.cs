#nullable disable
using System.Collections.ObjectModel;

namespace ControYaApp.Models
{
    public class OrdenProduccionCabecera
    {

        public string Centro { get; set; }

        public string CodigoProduccion { get; set; }

        public int Orden { get; set; }

        public DateTime Fecha { get; set; }

        public string Referencia { get; set; }

        public ObservableCollection<OrdenProduccionDetalle> Detalles { get; set; }
    }
}
