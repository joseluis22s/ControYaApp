﻿using System.Collections.ObjectModel;
using ControYaApp.Models;
using SQLite;

namespace ControYaApp.Services.LocalDatabase.Repositories
{
    public class OrdenProduccionMpRepo
    {
        private SQLiteAsyncConnection? _database;

        async Task InitAsync()
        {
            if (_database is not null)
                return;

            _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            await _database.CreateTableAsync<OrdenProduccionMp>();
        }

        // Retorna TRUE si el usuario se guardó. FALSE si el usuario no se guardó.
        public async Task SaveAllOrdenesProduccionPmAsync(ObservableCollection<OrdenProduccionMp> ordenesProduccionPm)
        {
            try
            {
                await InitAsync();

                await _database.DeleteAllAsync<OrdenProduccionMp>();

                await _database.InsertAllAsync(ordenesProduccionPm);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ObservableCollection<OrdenProduccionMp>> GetAllMaterialEgresado()
        {
            try
            {
                await InitAsync();

                var materiales = await _database.Table<OrdenProduccionMp>().ToListAsync();
                if (materiales.Count != 0)
                {
                    return new ObservableCollection<OrdenProduccionMp>(materiales);
                }
                return [];
            }
            catch (Exception) { throw; }
        }
    }
}
