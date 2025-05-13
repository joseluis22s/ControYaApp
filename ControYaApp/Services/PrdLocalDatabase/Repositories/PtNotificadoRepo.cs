
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

        public async Task SaveAllUnapprPtNotficado(List<PtNotificado> unapprPtNotificados)
        {
            try
            {
                await InitAsync();

                await _database.InsertAllAsync(unapprPtNotificados);
            }
            catch (Exception) { throw; }
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

        public async Task<List<PtNotificado>> GetAllPtNotificadosAsync()
        {
            try
            {
                await InitAsync();
                return await _database.Table<PtNotificado>().ToListAsync();
            }
            catch (Exception) { throw; }
        }
        public async Task<List<PtNotificado>> GetPtNotificadosNoSyncAsync()
        {
            try
            {
                await InitAsync();
                return await _database.Table<PtNotificado>().Where(pt => pt.Sincronizado == false).ToListAsync();
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

        public async Task<List<PtNotificado>> GetUnapprPtNotificadosPrd()
        {
            try
            {
                await InitAsync();
                return await _database.Table<PtNotificado>().Where(ptNotificado =>
                    ptNotificado.Sincronizado == true &&
                    ptNotificado.AprobarAutoProduccion == false
                ).ToListAsync();
            }
            catch (Exception) { throw; }
        }
        public async Task<List<PtNotificado>> GetUnapprPtNotificadosInv()
        {
            try
            {
                await InitAsync();
                return await _database.Table<PtNotificado>().Where(ptNotificado =>
                    ptNotificado.Sincronizado == true &&
                    ptNotificado.AprobarAutoProduccion == true &&
                    ptNotificado.AprobarAutoInventario == false
                ).ToListAsync();
            }
            catch (Exception) { throw; }
        }

        public async Task DeleteSyncApprovedPtNotificado()
        {
            try
            {
                await InitAsync();
                await _database.Table<PtNotificado>().DeleteAsync(pt =>
                    pt.Sincronizado == true
                );
            }
            catch (Exception) { throw; }

        }
    }
}
