using ControYaApp.Models;
using SQLite;

namespace ControYaApp.Services.LocalDatabase.Repositories
{
    public class UsuarioRepo
    {
        private SQLiteAsyncConnection? _database;

        public UsuarioRepo()
        {
        }

        async Task InitAsync()
        {
            if (_database is not null)
                return;

            _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            await _database.CreateTableAsync<Usuario>();
        }


        // Retorna TRUE si el usuario se guardó. FALSE si el usuario no se guardó.
        public async Task<bool> SaveUsuarioAsync(Usuario usuario)
        {
            try
            {
                await InitAsync();
                if (_database.Table<Usuario>().Equals(usuario))
                {
                    return false;
                }

                await _database.InsertAsync(usuario);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        // Retorna TRUE si credenciales son correctas. FALSE no son correctas.
        public async Task<string> CheckUsuarioCredentialsAsync(Usuario usuario)
        {
            try
            {
                await InitAsync();

                var usuarioDb = await _database.Table<Usuario>().Where(u =>
                    u.NombreUsuario == usuario.NombreUsuario &&
                    u.Contrasena == usuario.Contrasena).FirstAsync();


                if (usuarioDb != null)
                {
                    return usuarioDb.UsuarioSistema;
                }
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }



    }
}
