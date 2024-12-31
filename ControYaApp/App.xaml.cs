using ControYaApp.Services.Database;
using ControYaApp.UserExperience.Interfaces;
using Microsoft.Data.SqlClient;

namespace ControYaApp
{
    public partial class App : Application
    {
        public static IServiceProvider? Services;
        public static IAlertService? AlertSvc;
        public App(IServiceProvider provider)
        {
            InitializeComponent();
            Services = provider;
            AlertSvc = Services.GetService<IAlertService>();
            IniciarConexionDatabase();
        }

        private void IniciarConexionDatabase()
        {

            string server = "192.168.47.4";//"ipServidor/nombreServidor";
            string nombreDatabase = "POLLOSCRIOLLOCIA";//"nombreBasedeDatos";
            string usuario = "sa";//"usuario";
            string contrasena = "sa2025"; //"contrasena";

            string cadenaConexion = $"Server={server};Database={nombreDatabase};User Id={usuario};Password={contrasena};";

            DatabaseConnection databaseConnection = new DatabaseConnection(cadenaConexion);

            try
            {
                // El bloque using se encarga que las conexiones y otros recursos se liberen adecuadamente
                // después de su uso, evitando posibles fugas de memoria o bloqueos de recursos.
                using (SqlConnection conexion = databaseConnection.ConectarDatabase())
                {
                    Console.WriteLine("Conexión a la base de datos establecida exitosamente.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al usar la base de datos: {ex.Message}");
                throw;
            }

        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}