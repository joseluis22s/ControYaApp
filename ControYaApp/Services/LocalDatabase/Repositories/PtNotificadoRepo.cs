
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

        public async Task SaveOrUpdatePtNotificadoAsync(PtNotificado ptNotificado)
        {
            try
            {
                await InitAsync();
                var ptNotificadoSaved = await _database.Table<PtNotificado>().Where(pt =>
                    pt.CodigoProduccion == ptNotificado.CodigoProduccion &&
                    pt.Orden == ptNotificado.Orden &&
                    pt.CodigoMaterial == ptNotificado.CodigoMaterial &&
                    pt.Usuario == ptNotificado.Usuario
                ).FirstOrDefaultAsync();
                if (ptNotificadoSaved is not null)
                {
                    ptNotificadoSaved.AprobarAutoProduccion = ptNotificado.AprobarAutoProduccion;
                    ptNotificadoSaved.AprobarAutoInventario = ptNotificado.AprobarAutoInventario;
                    ptNotificadoSaved.Fecha = ptNotificado.Fecha;
                    ptNotificadoSaved.Notificado = ptNotificado.Notificado;
                    ptNotificadoSaved.CodigoEmpleado = ptNotificado.CodigoEmpleado;
                    ptNotificadoSaved.Serie = ptNotificado.Serie;
                    await _database.UpdateAsync(ptNotificadoSaved);
                }
                else
                {
                    await _database.InsertAsync(ptNotificado);
                }
            }
            catch (Exception) { throw; }
        }

        //public async Task SavePtNotificadoAsync(PtNotificado ptNotificado)
        //{
        //    try
        //    {
        //        await InitAsync();
        //        await _database.InsertAsync(ptNotificado);

        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public async Task SynchronizedFalsePtNotificadoAsync(PtNotificado ptNotificado)
        //{
        //    try
        //    {
        //        await InitAsync();
        //        var producto = await _database.Table<PtNotificadoReq>().Where(pt =>
        //            pt.CodigoProduccion == ptNotificado.CodigoProduccion &&
        //            pt.Orden == ptNotificado.Orden &&
        //            pt.CodigoMaterial == ptNotificado.CodigoMaterial &&
        //            pt.Usuario == pt.Usuario
        //        ).FirstOrDefaultAsync();
        //        if (producto is not null)
        //        {
        //            producto.Sincronizado = false;
        //            await _database.UpdateAsync(producto);
        //            return;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public async Task UpdateSincPtNotificadoAsync()
        //{
        //    try
        //    {
        //        await InitAsync();
        //        var productos = await _database.Table<PtNotificadoReq>().Where(pt => pt.Sincronizado == false).ToListAsync();
        //        foreach (var producto in productos)
        //        {
        //            producto.Sincronizado = true;
        //            await _database.UpdateAsync(producto);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public async Task SynchronizedTruePtNotificadoAsync(PtNotificado ptNotificado)
        //{
        //    try
        //    {
        //        await InitAsync();
        //        var producto = await _database.Table<PtNotificado>().Where(pt =>
        //            pt.CodigoProduccion == ptNotificado.CodigoProduccion &&
        //            pt.Orden == ptNotificado.Orden &&
        //            pt.CodigoMaterial == ptNotificado.CodigoMaterial &&
        //            pt.Usuario == pt.Usuario
        //        ).FirstOrDefaultAsync();

        //        if (producto is not null)
        //        {
        //            producto.Notificado = ptNotificado.Notificado;
        //            producto.Sincronizado = true;

        //            await _database.UpdateAsync(producto);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

    }
}
