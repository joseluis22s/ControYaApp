using ControYaApp.Services.Database;

namespace ControYaApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            IniciarConexionDatabase();
        }

        private void IniciarConexionDatabase()
        {

            string server = "ipServidor/nombreServidor";
            string nombreDatabase = "nombreBasedeDatos";
            string usuario = "usuario";
            string contrasena = "contrasena";

            string cadenaConexion = $"Server={server};Database={nombreDatabase};User Id={usuario};Password={contrasena};";

            DatabaseConnection databaseConnection = new DatabaseConnection(cadenaConexion);

            try
            {
                var conexion = databaseConnection.ConectarDatabase();
                Console.WriteLine("Conexión a la base de datos establecida exitosamente.");
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