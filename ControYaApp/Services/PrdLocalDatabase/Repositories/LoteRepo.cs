using CbMovil.Models;
using ControYaApp.Services.LocalDatabase;
using SQLite;

namespace CbMovil.Services.PrdLocalDatabase.Repositories
{
    public class LoteRepo
    {
        private SQLiteAsyncConnection? _database;

        async Task InitAsync()
        {
            if (_database is not null)
                return;

            _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            await _database.CreateTableAsync<Lote>();
        }

        public async Task SaveAllLoteAsync(List<Lote> lotes)
        {
            try
            {
                await InitAsync();
                await _database.DeleteAllAsync<Lote>();
                await _database.InsertAllAsync(lotes);
            }
            catch (Exception) { throw; }
        }


        public async Task SaveLoteAsync(Lote lote)
        {
            try
            {
                await InitAsync();

                await _database.InsertAsync(lote);
            }
            catch (Exception) { throw; }
        }

        public async Task<List<Lote>> GetAllLotesAsync()
        {
            try
            {
                await InitAsync();
                return await _database.Table<Lote>().ToListAsync();
            }
            catch (Exception) { throw; }
        }

        public async Task<List<Lote>> GetLotesNoSyncAsync()
        {
            try
            {
                await InitAsync();
                return await _database.Table<Lote>().Where(l => l.Sincronizar == true).ToListAsync();
            }
            catch (Exception) { throw; }
        }

        public async Task<bool> FindByNombre(string nombreLote)
        {
            try
            {
                await InitAsync();
                if (await _database.Table<Lote>().Where(l => l.Nombre == nombreLote).CountAsync() != 0)
                    return true;
            }
            catch (Exception) { throw; }
            return false;
        }
    }
}
