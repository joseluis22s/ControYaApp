using ControYaApp.Models;
using SQLite;

namespace ControYaApp.Services.LocalDatabase.Repositories
{
    public class IpServidorRepo
    {
        private SQLiteAsyncConnection? _database;

        async Task InitAsync()
        {
            if (_database is not null)
                return;

            _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            await _database.CreateTableAsync<IpServidor>();
        }

        public async Task SaveIpServidor(IpServidor ip)
        {
            try
            {
                await InitAsync();

                await _database.DeleteAllAsync<IpServidor>();

                await _database.InsertAsync(ip);
            }
            catch (Exception) { throw; }
        }


        public async Task<string> GetIpServidorAsync()
        {
            try
            {
                await InitAsync();
                var ip = await _database.Table<IpServidor>().FirstOrDefaultAsync();
                if (ip is not null)
                {
                    return ip.Ip;
                }

            }
            catch (Exception) { throw; }
            return "";
        }
    }
}
