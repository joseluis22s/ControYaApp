namespace ControYaApp.Models
{
    public class OrdenProduccionMaterialGroup : List<OrdenProduccionMp>
    {
        public OrdenProduccion OrdenProduccion { get; private set; }

        public string CodigoMaterial { get; set; }

        public string Material { get; set; }

        public OrdenProduccionMaterialGroup(OrdenProduccion ordenProduccion, List<OrdenProduccionMp> ordenesProduccionMp) : base(ordenesProduccionMp)
        {
            OrdenProduccion = ordenProduccion;
        }
    }
}
