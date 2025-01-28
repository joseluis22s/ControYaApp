using System.Collections.ObjectModel;
using ControYaApp.Models;
using ControYaApp.Services.WebService;
using SQLite;

namespace ControYaApp.Services.LocalDatabase.Repositories
{
    public class OrdenRepo
    {

        private readonly RestService _restService;
        private SQLiteAsyncConnection? _database;

        public OrdenRepo(RestService restService)
        {
            _restService = restService;
        }

        async Task InitAsync()
        {
            if (_database is not null)
                return;

            _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            var result = await _database.CreateTableAsync<OrdenProduccion>();
        }

        public async Task SaveOrdenesAsync(ObservableCollection<OrdenProduccion> ordenes)
        {
            try
            {
                await InitAsync();

                await _database.InsertAllAsync(ordenes);
            }
            catch (Exception)
            {
                return;
            }
        }

        public async Task<ObservableCollection<OrdenProduccion>> GetOrdenesByUsuario(string? nombreUsuario)
        {
            try
            {
                await InitAsync();
                var ordenes = await _database.Table<OrdenProduccion>().Where(t => t.CodigoUsuario.Equals(nombreUsuario)).ToListAsync();
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
    }
}
