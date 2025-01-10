using Microsoft.Data.SqlClient;

namespace ControYaApp.Services.Database
{
    public class DatabaseConnection
    {
        private readonly string _cadenaConexion;

        public DatabaseConnection()
        {
            string server = "SoporteJ";
            string nombreDatabase = "POLLOSCRIOLLOCIA";
            string usuario = "sa";
            string contrasena = "sa2025";

            string cadenaConexion = $"Server={server};Database={nombreDatabase};User Id={usuario};Password={contrasena};";

            _cadenaConexion = cadenaConexion;

        }

        public SqlConnection GetConexionDatabase()
        {
            return new SqlConnection(_cadenaConexion);
        }

        public bool ConectarDatabase()
        {
            try
            {
                using (SqlConnection conexionDatabase = GetConexionDatabase())
                {
                    conexionDatabase.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al conectar a la base de datos: {ex.Message}");
                var a = ex.Message;
                return false;
            }
        }

    }
}
