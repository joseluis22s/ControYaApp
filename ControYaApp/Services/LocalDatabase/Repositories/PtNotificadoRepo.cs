using ControYaApp.Services.WebService.ModelReq;
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

        public async Task SaveUpdatePtNotificadoAsync(PtNotificado ptNotificado)
        {
            try
            {
                await InitAsync();
                var producto = await _database.Table<PtNotificado>().Where(pt =>
                    pt.CodigoProduccion == ptNotificado.CodigoProduccion &&
                    pt.Orden == ptNotificado.Orden &&
                    pt.CodigoMaterial == ptNotificado.CodigoMaterial &&
                    pt.Usuario == pt.Usuario
                ).FirstOrDefaultAsync();

                NetworkAccess accessType = Connectivity.Current.NetworkAccess;

                // TODO: Refactorizar. A lo mejor hay que hcaer uqe en en donde se llame
                //                     a este metodo reescribir el sincronizado.
                if (producto is not null)
                {
                    if (accessType == NetworkAccess.Internet)
                    {
                        producto.Sincronizado = true;
                    }
                    else
                    {

                        producto.Sincronizado = false;
                    }
                    await _database.UpdateAsync(producto);
                    return;
                }
                if (accessType == NetworkAccess.Internet)
                {
                    ptNotificado.Sincronizado = true;
                }
                else
                {

                    ptNotificado.Sincronizado = false;
                }
                await _database.InsertAsync(ptNotificado);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateSincPtNotificadoAsync()
        {
            try
            {
                await InitAsync();
                var productos = await _database.Table<PtNotificado>().Where(pt => pt.Sincronizado == false).ToListAsync();
                foreach (var producto in productos)
                {
                    producto.Sincronizado = true;
                    await _database.UpdateAsync(producto);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SetSincPtNotificadoAsync(PtNotificado ptNotificado)
        {
            try
            {
                await InitAsync();
                var producto = await _database.Table<PtNotificado>().Where(pt =>
                    pt.CodigoProduccion == ptNotificado.CodigoProduccion &&
                    pt.Orden == ptNotificado.Orden &&
                    pt.CodigoMaterial == ptNotificado.CodigoMaterial &&
                    pt.Usuario == pt.Usuario
                ).FirstOrDefaultAsync();

                if (producto is not null)
                {
                    producto.Sincronizado = true;

                    await _database.UpdateAsync(producto);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
