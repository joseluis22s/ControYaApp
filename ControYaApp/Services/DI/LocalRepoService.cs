using ControYaApp.Services.LocalDatabase.Repositories;

namespace ControYaApp.Services.DI
{
    public class LocalRepoService
    {
        public EmpleadosRepo EmpleadosRepo { get; }

        public MaterialEgresadoRepo MaterialEgresadoRepo { get; }

        public ProductoTerminadoRepo ProductoTerminadoRepo { get; }

        public PeriodoRepo PeriodoRepo { get; }

        public OrdenRepo OrdenRepo { get; }

        public UsuarioRepo UsuarioRepo { get; }


        public LocalRepoService(EmpleadosRepo empleadosRepo,
            MaterialEgresadoRepo materialEgresadoRepo,
            ProductoTerminadoRepo productoTerminadoRepo,
            PeriodoRepo periodoRepo,
            OrdenRepo ordenRepo,
            UsuarioRepo usuarioRepo)
        {
            EmpleadosRepo = empleadosRepo;
            MaterialEgresadoRepo = materialEgresadoRepo;
            ProductoTerminadoRepo = productoTerminadoRepo;
            PeriodoRepo = periodoRepo;
            OrdenRepo = ordenRepo;
            UsuarioRepo = usuarioRepo;
        }
    }
}
