using System.Collections.ObjectModel;
using ControYaApp.Models;
using SQLite;

namespace ControYaApp.Services.LocalDatabase.Repositories
{
    public class OrdenProduccionPtRepo
    {
        private SQLiteAsyncConnection? _database;

        async Task InitAsync()
        {
            if (_database is not null)
                return;

            _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            await _database.CreateTableAsync<OrdenProduccionPt>();
        }

        // Retorna TRUE si el usuario se guardó. FALSE si el usuario no se guardó.
        public async Task SaveAllOrdenesProduccionPtAsync(ObservableCollection<OrdenProduccionPt> ordenesProduccionPt)
        {
            try
            {
                await InitAsync();

                await _database.DeleteAllAsync<OrdenProduccionPt>();

                await _database.InsertAllAsync(ordenesProduccionPt);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<OrdenProduccionPt>> GetAllOrdenesProduccionPt()
        {
            try
            {
                await InitAsync();

                var productos = await _database.Table<OrdenProduccionPt>().ToListAsync();
                if (productos.Count != 0)
                {
                    return new ObservableCollection<OrdenProduccionPt>(productos);
                }
                return [];
            }
            catch (Exception) { throw; }
        }


    }
}
