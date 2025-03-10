
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
            await _database.CreateTableAsync<PtNotificadoReq>();
        }

        public async Task SynchronizedFalsePtNotificadoAsync(PtNotificadoReq ptNotificado)
        {
            try
            {
                await InitAsync();
                var producto = await _database.Table<PtNotificadoReq>().Where(pt =>
                    pt.CodigoProduccion == ptNotificado.CodigoProduccion &&
                    pt.Orden == ptNotificado.Orden &&
                    pt.CodigoMaterial == ptNotificado.CodigoMaterial &&
                    pt.Usuario == pt.Usuario
                ).FirstOrDefaultAsync();
                if (producto is not null)
                {
                    producto.Sincronizado = false;
                    await _database.UpdateAsync(producto);
                    return;
                }
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
                var productos = await _database.Table<PtNotificadoReq>().Where(pt => pt.Sincronizado == false).ToListAsync();
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

        public async Task SynchronizedTruePtNotificadoAsync(PtNotificadoReq ptNotificado)
        {
            try
            {
                await InitAsync();
                var producto = await _database.Table<PtNotificadoReq>().Where(pt =>
                    pt.CodigoProduccion == ptNotificado.CodigoProduccion &&
                    pt.Orden == ptNotificado.Orden &&
                    pt.CodigoMaterial == ptNotificado.CodigoMaterial &&
                    pt.Usuario == pt.Usuario
                ).FirstOrDefaultAsync();

                if (producto is not null)
                {
                    producto.Notificado = ptNotificado.Notificado;
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
