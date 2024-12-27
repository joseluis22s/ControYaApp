using Microsoft.Data.SqlClient;

namespace ControYaApp.Database
{
    public class DatabaseConnection
    {
        private SqlConnection? conn;

        private void connectToDatabase()
        {
            string ipServidor = "https://www.youtube.com/watch?v=xNmIdFjXzl4";


            string connStr = $"Data Sorce={}\MSSQLLocalDB;Initial Catalog=exam_prototypingDB;Integrated Security=True";
        }
    }
}
