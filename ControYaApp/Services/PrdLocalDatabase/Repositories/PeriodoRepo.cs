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
        public async Task SaveRangosPeriodosAsync(Periodos periodos)
        {
            try
            {
                await InitAsync();

                await _database.DeleteAllAsync<Periodos>();

                await _database.InsertAsync(periodos);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Periodos?> GetRangosPeriodosAsync()
        {
            Periodos periodos = new Periodos();
            try
            {
                await InitAsync();
                periodos = await _database.Table<Periodos>().FirstOrDefaultAsync();
                if (periodos is not null)
                {
                    return periodos;
                }
            }
            catch (Exception) { throw; }
            return periodos;
        }


    }
}
