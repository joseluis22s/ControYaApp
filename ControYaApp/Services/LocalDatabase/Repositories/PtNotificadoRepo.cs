
using ControYaApp.Models;
using SQLite;

namespace ControYaApp.Services.LocalDatabase.Repositories
{
    public class PtNotificadoRepo
    {
        private SQLiteAsyncConnection? _database;

        async Task InitAsync()
        {
            if (_database is not null)
                return;

            _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            await _database.CreateTableAsync<PtNotificado>();
        }

        public async Task SavePtNotificadoAsync(PtNotificado ptNotificado)
        {
            try
            {
                await InitAsync();

                await _database.InsertAsync(ptNotificado);
            }
            catch (Exception) { throw; }
        }

        public async Task<List<PtNotificado>> GetAllPtNotificadoAsync()
        {
            try
            {
                await InitAsync();
                return await _database.Table<PtNotificado>().ToListAsync();
            }
            catch (Exception) { throw; }
        }

        public async Task DeleteAllPtNotificado()
        {
            try
            {
                await InitAsync();

                await _database.DeleteAllAsync<PtNotificado>();
            }
            catch (Exception) { throw; }
        }

    }
}
