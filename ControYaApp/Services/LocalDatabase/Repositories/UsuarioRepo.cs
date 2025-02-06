using System.Collections.ObjectModel;
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
            // TODO: Eliminar el retorno bool del método.
            try
            {
                await InitAsync();
                var count = await _database.Table<Usuario>().CountAsync(tbu =>
                    tbu.UsuarioSistema == usuario.UsuarioSistema &&
                    tbu.NombreUsuario == usuario.NombreUsuario &&
                    tbu.Contrasena == usuario.Contrasena);
                if (count == 0)
                {
                    await _database.InsertAsync(usuario);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }


        // Retorna TRUE si credenciales son correctas. FALSE no son correctas.
        public async Task<Dictionary<string, object>> CheckUsuarioCredentialsAsync(Usuario usuario)
        {
            try
            {
                // TODO: Verificar si 'usuarioDb' puede ser 'null'. En ese caso el return cambio
                await InitAsync();


                var usuarioDb = await _database.Table<Usuario>().Where(u =>
                    u.NombreUsuario.Equals(usuario.NombreUsuario) &&
                    u.Contrasena.Equals(usuario.Contrasena)).FirstOrDefaultAsync();

                if (usuarioDb != null)
                {
                    return new Dictionary<string, object>
                    {
                        {"usuarioSistema" , usuarioDb.UsuarioSistema},
                        {"estaRegistrado" , true }
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }

            return new Dictionary<string, object>
                    {
                        {"usuarioSistema" , ""},
                        {"estaRegistrado" , false }
                    };
        }

        public async Task SaveAllUsuariosAsync(ObservableCollection<Usuario> usuarios)
        {
            try
            {
                await InitAsync();

                await _database.DeleteAllAsync<Usuario>();

                await _database.InsertAllAsync(usuarios);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<Usuario>> GetAllUsuarios()
        {
            try
            {
                await InitAsync();

                var usuarios = await _database.Table<Usuario>().ToListAsync();
                if (usuarios.Count != 0)
                {
                    return new ObservableCollection<Usuario>(usuarios);
                }
                return [];
            }
            catch (Exception) { throw; }
        }


    }
}
