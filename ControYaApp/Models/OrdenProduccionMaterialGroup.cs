using System.Collections.ObjectModel;

namespace ControYaApp.Models
{
    public class OrdenProduccionMaterialGroup : ObservableCollection<OrdenProduccionMp>
    {
        public string CodigoProduccion { get; set; }

        public int Orden { get; set; }

        public string CodigoMaterial { get; set; }

        public string Material { get; set; }

        public OrdenProduccionMaterialGroup(string codigoProduccion, int orden, string codigoMaterial, string material, ObservableCollection<OrdenProduccionMp> ordenesProduccionMp) : base(ordenesProduccionMp)
        {
            CodigoProduccion = codigoProduccion;
            Orden = orden;
            CodigoMaterial = codigoMaterial;
            Material = material;
        }
    }
}
