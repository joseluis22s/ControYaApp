using Microsoft.Data.SqlClient;

namespace ControYaApp.Services.Database
{
    public class DatabaseConnection
    {

        private readonly string _cadenaConexion;

        public DatabaseConnection(string cadenaConexion)
        {
            if (string.IsNullOrEmpty(cadenaConexion))
            {
                throw new ArgumentException("La cadena de conexión no puede estar vacía.");
            }

            _cadenaConexion = cadenaConexion;
        }

        public SqlConnection ConectarDatabase()
        {
            try
            {
                SqlConnection conexion = new SqlConnection(_cadenaConexion);
                conexion.Open();
                return conexion;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al conectar a la base de datos: {ex.Message}");
                throw;
            }
        }
    }
}
