using CbMovil.Services.PrdLocalDatabase.Repositories;
using ControYaApp.Services.LocalDatabase.Repositories;

namespace ControYaApp.Services.LocalDatabase
{
    public class PrdDbReposService
    {
        public OrdenProduccionRepo OrdenProduccionRepo { get; }

        public OrdenProduccionPtRepo OrdenProduccionPtRepo { get; }

        public OrdenProduccionMpRepo OrdenProduccionMpRepo { get; }

        public PtNotificadoRepo PtNotificadoRepo { get; }

        public MpNotificadoRepo MpNotificadoRepo { get; }

        public LoteRepo LoteRepo { get; }


        public PrdDbReposService(OrdenProduccionRepo ordenProduccionRepo,
            OrdenProduccionPtRepo ordenProduccionPtRepo, OrdenProduccionMpRepo ordenProduccionMpRepo,
            PtNotificadoRepo ptNotificadoRepo, MpNotificadoRepo mpNotificadoRepo,
            LoteRepo loteRepo)
        {
            OrdenProduccionRepo = ordenProduccionRepo;
            OrdenProduccionPtRepo = ordenProduccionPtRepo;
            OrdenProduccionMpRepo = ordenProduccionMpRepo;
            PtNotificadoRepo = ptNotificadoRepo;
            MpNotificadoRepo = mpNotificadoRepo;
            LoteRepo = loteRepo;
        }
    }
}
