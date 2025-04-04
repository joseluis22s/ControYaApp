using ControYaApp.Services.LocalDatabase.Repositories;

namespace ControYaApp.Services.AppLocalDatabase
{
    public class AppDbReposService
    {
        public DataConfigRepo DataConfigRepo { get; }

        public EmpleadosRepo EmpleadosRepo { get; }

        public PeriodoRepo PeriodoRepo { get; }

        public UsuarioRepo UsuarioRepo { get; }

        public AppDbReposService(DataConfigRepo dataConfigRepo, EmpleadosRepo empleadosRepo,
            PeriodoRepo periodoRepo, UsuarioRepo usuarioRepo)
        {
            DataConfigRepo = dataConfigRepo;
            EmpleadosRepo = empleadosRepo;
            PeriodoRepo = periodoRepo;
            UsuarioRepo = usuarioRepo;
        }
    }
}
