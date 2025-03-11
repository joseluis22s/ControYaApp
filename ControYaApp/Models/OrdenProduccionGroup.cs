namespace ControYaApp.Models
{
    public class OrdenProduccionGroup : List<OrdenProduccionPt>
    {
        public OrdenProduccion OrdenProduccion { get; private set; }

        public OrdenProduccionGroup(OrdenProduccion ordenProduccion, List<OrdenProduccionPt> ordenesProduccionPt) : base(ordenesProduccionPt)
        {
            OrdenProduccion = ordenProduccion;
        }
    }
}
