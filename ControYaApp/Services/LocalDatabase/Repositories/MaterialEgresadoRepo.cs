using System.Collections.ObjectModel;
using ControYaApp.Models;
using SQLite;

namespace ControYaApp.Services.LocalDatabase.Repositories
{
    public class MaterialEgresadoRepo
    {
        private SQLiteAsyncConnection? _database;

        async Task InitAsync()
        {
            if (_database is not null)
                return;

            _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            await _database.CreateTableAsync<NotificarEm>();
        }

        // Retorna TRUE si el usuario se guardó. FALSE si el usuario no se guardó.
        public async Task SaveAllEmAsync(ObservableCollection<NotificarEm> materiales)
        {
            try
            {
                await InitAsync();

                await _database.DeleteAllAsync<NotificarEm>();

                await _database.InsertAllAsync(materiales);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<NotificarEm>> GetAllMaterialEgresado()
        {
            try
            {
                await InitAsync();

                var materiales = await _database.Table<NotificarEm>().ToListAsync();
                if (materiales.Count != 0)
                {
                    return new ObservableCollection<NotificarEm>(materiales);
                }
                return [];
            }
            catch (Exception) { throw; }
        }
    }
}
