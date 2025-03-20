using System.Collections.ObjectModel;
using ControYaApp.Models;
using SQLite;

namespace ControYaApp.Services.LocalDatabase.Repositories
{
    public class OrdenProduccionMpRepo
    {
        private SQLiteAsyncConnection? _database;

        async Task InitAsync()
        {
            if (_database is not null)
                return;

            _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            await _database.CreateTableAsync<OrdenProduccionMp>();
        }

        public async Task SaveAllOrdenesProduccionPmAsync(ObservableCollection<OrdenProduccionMp> ordenesProduccionMp)
        {
            try
            {
                await InitAsync();

                await _database.DeleteAllAsync<OrdenProduccionMp>();

                await _database.InsertAllAsync(ordenesProduccionMp);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<OrdenProduccionMp>> GetOrdenesProduccionMpByOrdenProduccion(OrdenProduccion ordenProduccion)
        {
            try
            {
                await InitAsync();

                var ordenesProduccionMp = await _database.Table<OrdenProduccionMp>()
                    .Where(orMp =>
                        orMp.Orden == ordenProduccion.Orden &&
                        orMp.CodigoProduccion == ordenProduccion.CodigoProduccion
                    ).ToListAsync();
                if (ordenesProduccionMp.Count != 0)
                {
                    return new ObservableCollection<OrdenProduccionMp>(ordenesProduccionMp);
                }
                return [];
            }
            catch (Exception) { throw; }
        }

        public async Task UpdateNotificadoAsync(OrdenProduccionMp ordenProduccionMp)
        {
            try
            {
                await InitAsync();

                // TODO: En caso de error, buscar con Any y el argumento pra obtener la pk
                await _database.UpdateAsync(ordenProduccionMp);
            }
            catch (Exception) { throw; }
        }
    }
}
