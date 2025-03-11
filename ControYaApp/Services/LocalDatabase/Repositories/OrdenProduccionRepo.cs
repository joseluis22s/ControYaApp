using System.Collections.ObjectModel;
using ControYaApp.Models;
using SQLite;

namespace ControYaApp.Services.LocalDatabase.Repositories
{
    public class OrdenProduccionRepo
    {
        private SQLiteAsyncConnection? _database;


        // TODO: Eliminar todo lo que no se use.

        async Task InitAsync()
        {
            if (_database is not null)
                return;

            _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            await _database.CreateTableAsync<OrdenProduccion>();
        }

        public async Task<decimal> GetOrdenNotificadaAsync(OrdenProduccion orden)
        {
            try
            {
                await InitAsync();

                var valorNotificado = await _database.Table<OrdenProduccion>().Where(o =>
                    o.CodigoProduccion == orden.CodigoProduccion &&
                    o.Orden == orden.Orden &&
                    o.CodigoMaterial == orden.CodigoMaterial &&
                    o.CodigoUsuario == orden.CodigoUsuario
                ).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
            return 0;
        }

        public async Task SaveOrdenesAsync(ObservableCollection<OrdenProduccion> ordenes)
        {
            try
            {
                await InitAsync();

                var UnSavedOrdenes = new ObservableCollection<OrdenProduccion>();

                foreach (var orden in ordenes)
                {
                    var count = await _database.Table<OrdenProduccion>().CountAsync(tbu =>
                        tbu.Centro == orden.Centro &&
                        tbu.CodigoProduccion == orden.CodigoProduccion &&
                        tbu.Orden == orden.Orden &&
                        tbu.CodigoUsuario == orden.CodigoUsuario &&
                        tbu.Fecha == orden.Fecha &&
                        tbu.Referencia == orden.Referencia &&
                        tbu.Detalle == orden.Detalle &&
                        tbu.CodigoMaterial == orden.CodigoMaterial &&
                        tbu.CodigoProducto == orden.CodigoProducto &&
                        tbu.Producto == orden.Producto &&
                        tbu.CodigoUnidad == orden.CodigoUnidad &&
                        tbu.Cantidad == orden.Cantidad &&
                        tbu.Notificado == orden.Notificado);
                    if (count == 0)
                    {
                        UnSavedOrdenes.Add(orden);
                    }
                }

                await _database.InsertAllAsync(UnSavedOrdenes);
            }
            catch (Exception)
            {
                return;
            }
        }

        public async Task<ObservableCollection<OrdenProduccion>> GetOrdenesProduccionByUsuarioSistema(string? usuarioSistema)
        {
            try
            {
                await InitAsync();
                var ordenes = await _database.Table<OrdenProduccion>().Where(t => t.CodigoUsuarioAprobar.Equals(usuarioSistema)).ToListAsync();
                return new ObservableCollection<OrdenProduccion>(ordenes);
            }
            catch (Exception)
            {
                return [];
            }
        }

        public async Task DeletAllOrdenes()
        {
            try
            {
                await InitAsync();

                await _database.DeleteAllAsync<OrdenProduccion>();
            }
            catch (Exception)
            {
                return;
            }
        }

        public async Task SaveAllOrdenesProduccionAsync(ObservableCollection<OrdenProduccion> ordenesProduccion)
        {
            try
            {
                await InitAsync();

                await _database.DeleteAllAsync<OrdenProduccion>();

                await _database.InsertAllAsync(ordenesProduccion);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<OrdenProduccion>> GetAllOrdenes()
        {
            try
            {
                await InitAsync();

                var ordenes = await _database.Table<OrdenProduccion>().ToListAsync();
                if (ordenes.Count != 0)
                {
                    return new ObservableCollection<OrdenProduccion>(ordenes);
                }
                return [];
            }
            catch (Exception) { throw; }
        }

        public async Task NotificarProductoTerminadoAsync(PtNotificadoReq ptnotificado)
        {
            try
            {
                await InitAsync();

                var producto = await _database.Table<OrdenProduccion>().Where(pt =>
                    pt.CodigoProduccion == ptnotificado.CodigoProduccion &&
                    pt.Orden == ptnotificado.Orden &&
                    pt.CodigoMaterial == ptnotificado.CodigoMaterial &&
                    pt.CodigoUsuario == ptnotificado.Usuario
                ).FirstOrDefaultAsync();

                producto.Notificado = ptnotificado.Notificado;

                await _database.UpdateAsync(producto);


            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
