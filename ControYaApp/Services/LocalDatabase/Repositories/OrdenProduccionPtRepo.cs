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


        public async Task SaveAllOrdenesProduccionPtAsync(ObservableCollection<OrdenProduccionPt> ordenesProduccionPt)
        {
            try
            {
                await InitAsync();

                await _database.DeleteAllAsync<OrdenProduccionPt>();

                await _database.InsertAllAsync(ordenesProduccionPt);
            }
            catch (Exception) { throw; }
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
            }
            catch (Exception) { throw; }

            return [];
        }

        public async Task UpdateNotificadoAsync(OrdenProduccionPt ordenProduccionPt)
        {
            try
            {
                await InitAsync();

                // TODO: En caso de error, buscar con Any y el argumento pra obtener la pk
                await _database.UpdateAsync(ordenProduccionPt);
            }
            catch (Exception) { throw; }
        }

        public async Task<decimal?> GetNotificadoValue(OrdenProduccionPt ordenProduccionpt)
        {
            try
            {
                await InitAsync();

                var oppt = await _database.Table<OrdenProduccionPt>().FirstOrDefaultAsync(oppt =>
                        oppt.Centro == ordenProduccionpt.Centro &&
                        oppt.CodigoProduccion == ordenProduccionpt.CodigoProduccion &&
                        oppt.Orden == ordenProduccionpt.Orden &&
                        oppt.CodigoMaterial == ordenProduccionpt.CodigoMaterial &&
                        oppt.CodigoProducto == ordenProduccionpt.CodigoMaterial);

                return oppt.Notificado;
            }
            catch (Exception) { throw; }
        }



    }
}
