﻿using System.Collections.ObjectModel;
using ControYaApp.Models;
using SQLite;

namespace ControYaApp.Services.LocalDatabase.Repositories
{
    public class ProductoTerminadoRepo
    {
        private SQLiteAsyncConnection? _database;

        async Task InitAsync()
        {
            if (_database is not null)
                return;

            _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            await _database.CreateTableAsync<NotificarPt>();
        }

        // Retorna TRUE si el usuario se guardó. FALSE si el usuario no se guardó.
        public async Task SaveAllPtAsync(ObservableCollection<NotificarPt> productos)
        {
            try
            {
                await InitAsync();

                await _database.DeleteAllAsync<NotificarPt>();

                await _database.InsertAllAsync(productos);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<NotificarPt>> GetAllProductosT()
        {
            try
            {
                await InitAsync();

                var productos = await _database.Table<NotificarPt>().ToListAsync();
                if (productos.Count != 0)
                {
                    return new ObservableCollection<NotificarPt>(productos);
                }
                return [];
            }
            catch (Exception) { throw; }
        }


    }
}
