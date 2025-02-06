using System.Collections.ObjectModel;
using ControYaApp.Models;
using SQLite;

namespace ControYaApp.Services.LocalDatabase.Repositories
{
    public class PeriodoRepo
    {
        private SQLiteAsyncConnection? _database;

        async Task InitAsync()
        {
            if (_database is not null)
                return;

            _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            await _database.CreateTableAsync<Periodos>();
        }

        // Retorna TRUE si el usuario se guardó. FALSE si el usuario no se guardó.
        public async Task SaveAllPeriodosAsync(ObservableCollection<Periodos> periodos)
        {
            try
            {
                await InitAsync();

                await _database.DeleteAllAsync<Periodos>();

                await _database.InsertAllAsync(periodos);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<Periodos>> GetAllPeriodos()
        {
            try
            {
                await InitAsync();

                var periodos = await _database.Table<Periodos>().ToListAsync();
                if (periodos.Count != 0)
                {
                    return new ObservableCollection<Periodos>(periodos);
                }
                return [];
            }
            catch (Exception) { throw; }
        }

    }
}
