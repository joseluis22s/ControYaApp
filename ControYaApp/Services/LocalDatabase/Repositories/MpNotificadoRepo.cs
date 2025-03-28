using ControYaApp.Models;
using SQLite;

namespace ControYaApp.Services.LocalDatabase.Repositories
{
    public class MpNotificadoRepo
    {
        private SQLiteAsyncConnection? _database;

        async Task InitAsync()
        {
            if (_database is not null)
                return;

            _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            await _database.CreateTableAsync<MpNotificado>();
        }

        public async Task SaveAllUnapprMpNotficado(List<MpNotificado> unapprMpNotificados)
        {
            try
            {
                await InitAsync();

                await _database.InsertAllAsync(unapprMpNotificados);
            }
            catch (Exception) { throw; }
        }

        public async Task<int> SaveMpNotificadosAsync(List<MpNotificado> pmNotificados)
        {
            try
            {
                await InitAsync();
                return await _database.InsertAllAsync(pmNotificados);
            }
            catch (Exception) { throw; }
        }

        public async Task<List<MpNotificado>> GetAllMpNotificadoAsync()
        {
            try
            {
                await InitAsync();
                return await _database.Table<MpNotificado>().ToListAsync();
            }
            catch (Exception) { throw; }
        }

        public async Task DeleteAllMpNotificado()
        {
            try
            {
                await InitAsync();

                await _database.DeleteAllAsync<MpNotificado>();
            }
            catch (Exception) { throw; }
        }

        public async Task<List<MpNotificado>> GetUnapprMpNotificadosPrd()
        {
            try
            {
                await InitAsync();
                return await _database.Table<MpNotificado>().Where(mp =>
                    mp.Sincronizado == true &&
                    mp.AprobarAutoProduccion == false
                ).ToListAsync();
            }
            catch (Exception) { throw; }
        }
    }
}
