using ControYaApp.Models;
using SQLite;

namespace ControYaApp.Services.LocalDatabase.Repositories
{
    public class PmNotificadoRepo
    {
        private SQLiteAsyncConnection? _database;

        async Task InitAsync()
        {
            if (_database is not null)
                return;

            _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            await _database.CreateTableAsync<PmNotificado>();
        }

        public async Task<int> SaveOrUpdatePtNotificadoAsync(PmNotificado pmNotificado)
        {
            try
            {
                await InitAsync();
                var pmNotificadoSaved = await _database.Table<PmNotificado>().Where(pm =>
                    pm.Id == pmNotificado.Id
                ).FirstOrDefaultAsync();
                if (pmNotificadoSaved is not null)
                {
                    pmNotificadoSaved.NotificacionAutorizada = pmNotificado.NotificacionAutorizada;
                    pmNotificadoSaved.Fecha = pmNotificado.Fecha;
                    pmNotificadoSaved.Notificado = pmNotificado.Notificado;
                    pmNotificadoSaved.CodigoEmpleado = pmNotificado.CodigoEmpleado;
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
    }
}
