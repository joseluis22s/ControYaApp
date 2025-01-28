using ControYaApp.Models;
using SQLite;

namespace ControYaApp.Services.LocalDatabase.Repositories
{
    public class OrdenRepo
    {
        private SQLiteAsyncConnection? _database;

        public OrdenRepo()
        {
        }

        async Task InitAsync()
        {
            if (_database is not null)
                return;

            _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            var result = await _database.CreateTableAsync<OrdenProduccion>();
        }

        public async Task<ICollection<OrdenProduccion>> GetOrdenesByUsuario()
        {
            try
            {
                await InitAsync();

                return await _database.Table<OrdenProduccion>().ToListAsync();
            }
            catch (Exception)
            {
                return [];
            }
        }
    }
}
