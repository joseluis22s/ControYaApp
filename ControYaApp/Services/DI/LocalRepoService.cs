using ControYaApp.Services.LocalDatabase.Repositories;

namespace ControYaApp.Services.DI
{
    public class LocalRepoService(EmpleadosRepo empleadosRepo,
        OrdenProduccionMpRepo ordenProduccionMpRepo,
        OrdenProduccionPtRepo ordenProduccionPtRepo,
        PeriodoRepo periodoRepo,
        OrdenProduccionRepo ordenesProduccionRepo,
        UsuarioRepo usuarioRepo)
    {
        public EmpleadosRepo EmpleadosRepo { get; } = empleadosRepo;

        public OrdenProduccionMpRepo OrdenProduccionMpRepo { get; } = ordenProduccionMpRepo;

        public OrdenProduccionPtRepo OrdenProduccionPtRepo { get; } = ordenProduccionPtRepo;

        public PeriodoRepo PeriodoRepo { get; } = periodoRepo;

        public OrdenProduccionRepo OrdenesProduccionRepo { get; } = ordenesProduccionRepo;

        public UsuarioRepo UsuarioRepo { get; } = usuarioRepo;
    }
}
