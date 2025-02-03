using System.Collections.ObjectModel;
using ControYaApp.Models;
using SQLite;

namespace ControYaApp.Services.LocalDatabase.Repositories
{

    public class EmpleadosRepo
    {
        private SQLiteAsyncConnection? _database;

        async Task InitAsync()
        {
            if (_database is not null)
                return;

            _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            await _database.CreateTableAsync<EmpleadoSistema>();
        }

        // Retorna TRUE si el usuario se guardó. FALSE si el usuario no se guardó.
        public async Task SaveAllEmpleadosAsync(ObservableCollection<EmpleadoSistema> empleados)
        {
            try
            {
                await InitAsync();

                await _database.DeleteAllAsync<EmpleadoSistema>();

                await _database.InsertAllAsync(empleados);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
