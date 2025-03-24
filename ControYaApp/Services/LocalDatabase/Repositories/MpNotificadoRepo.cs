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

        public async Task<int> SaveOrUpdatePtNotificadoAsync(MpNotificado pmNotificado)
        {
            try
            {
                await InitAsync();
                var pmNotificadoSaved = await _database.Table<MpNotificado>().Where(pm =>
                    pm.Id == pmNotificado.Id
                ).FirstOrDefaultAsync();
                if (pmNotificadoSaved is not null)
                {
                    pmNotificadoSaved.AprobarAutoProduccion = pmNotificado.AprobarAutoProduccion;
                    pmNotificadoSaved.AprobarAutoInventario = pmNotificado.AprobarAutoInventario;
                    pmNotificadoSaved.Fecha = pmNotificado.Fecha;
                    pmNotificadoSaved.Notificado = pmNotificado.Notificado;
                    pmNotificadoSaved.CodigoUsuario = pmNotificado.CodigoUsuario;
                    return await _database.UpdateAsync(pmNotificadoSaved);
                }
                else
                {
                    return await _database.InsertAsync(pmNotificado);
                }
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

        //public async Task AuthorizeAllMpNotificado()
        //{
        //    try
        //    {
        //        await InitAsync();

        //        // Actualiza todos los registros en una sola operación
        //        await _database.ExecuteAsync(
        //            "UPDATE PmNotificado SET NotificacionAutorizada = ? WHERE NotificacionAutorizada = ?",
        //            true, false);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Maneja otras excepciones
        //        Console.WriteLine($"Error inesperado: {ex.Message}");
        //        throw;
        //    }
        //}

        //public async Task AuthorizeAllMpNotificado()
        //{
        //    try
        //    {
        //        await InitAsync();
        //        var mpNotificados = await _database.Table<PmNotificado>().Where(mp =>
        //            mp.NotificacionAutorizada == false
        //        ).ToListAsync();
        //        foreach (var mpNotificado in mpNotificados)
        //        {
        //            mpNotificado.NotificacionAutorizada = true;
        //            await _database.UpdateAsync(mpNotificado);
        //        }
        //    }
        //    catch (Exception) { throw; }
        //}
    }
}
