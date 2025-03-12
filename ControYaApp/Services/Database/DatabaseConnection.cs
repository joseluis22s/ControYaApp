using System.Diagnostics;

namespace ControYaApp.Services.Database
{
    public class DatabaseConnection
    {
        private readonly string _cadenaConexion;

        public DatabaseConnection()
        {
            string server = "192.168.47.4";
            string nombreDatabase = "POLLOSCRIOLLOCIA";
            string usuario = "sa";
            string contrasena = "sa2025";

            string cadenaConexion = $"Server={server};Database={nombreDatabase};User Id={usuario};Password={contrasena};TrustServerCertificate=true";

            _cadenaConexion = cadenaConexion;

        }

        public async Task<bool> ConectarDatabase()
        {
            using var conexionDatabase = new SqlConnection(_cadenaConexion);
            try
            {
                await conexionDatabase.OpenAsync();
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Error al conectar a la base de datos: {ex.Message}");
                return false;
            }
            finally
            {
                await conexionDatabase.CloseAsync();
            }
        }

    }
}
