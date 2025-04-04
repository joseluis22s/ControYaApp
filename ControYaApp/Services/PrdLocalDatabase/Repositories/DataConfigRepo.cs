using ControYaApp.Models;
using SQLite;

namespace ControYaApp.Services.LocalDatabase.Repositories
{
    public class DataConfigRepo
    {
        private SQLiteAsyncConnection? _database;

        async Task InitAsync()
        {
            if (_database is not null)
                return;

            _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            await _database.CreateTableAsync<DataConfig>();
        }

        public async Task SaveIpServidor(DataConfig dataConfig)
        {
            try
            {
                await InitAsync();

                await _database.DeleteAllAsync<DataConfig>();

                await _database.InsertAsync(dataConfig);
            }
            catch (Exception) { throw; }
        }


        public async Task<DataConfig?> GetDataConfigAsync()
        {
            DataConfig dataConfig = new();
            try
            {
                await InitAsync();
                dataConfig = await _database.Table<DataConfig>().FirstOrDefaultAsync();

                if (dataConfig is not null)
                {
                    return dataConfig;
                }
            }
            catch (Exception) { throw; }
            return dataConfig;
        }
    }
}
