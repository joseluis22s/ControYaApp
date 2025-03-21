
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
                    pt.CodigoUsuario == ptNotificado.CodigoUsuario
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

        public async Task<List<PtNotificado>> GetAllPtNotificadoAsync()
        {
            try
            {
                await InitAsync();
                return await _database.Table<PtNotificado>().ToListAsync();
            }
            catch (Exception) { throw; }
        }

        public async Task DeleteAllPtNotificado()
        {
            try
            {
                await InitAsync();

                await _database.DeleteAllAsync<EmpleadoSistema>();
            }
            catch (Exception) { throw; }
        }

    }
}
