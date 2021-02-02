﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XamarinProjects.Models;
using SQLite;
using System.IO;

namespace XamarinProjects.Repositories
{
    public class TodoItemRepository : ITodoItemRepository
    {
        private SQLiteAsyncConnection connection;

        public event EventHandler<TodoItem> OnItemAdded;
        public event EventHandler<TodoItem> OnItemUpdated;
        public event EventHandler<TodoItem> OnItemRemoved;

        public async Task AddItem(TodoItem item) {
            await CreateConnection();
            await connection.InsertAsync(item);
            OnItemAdded?.Invoke(this, item);
        }

        public async Task RemoveItem(TodoItem item) {
            await CreateConnection();
            await connection.DeleteAsync(item);
            OnItemRemoved?.Invoke(this, item);
        }

        public async Task UpdateItem(TodoItem item) {
            await CreateConnection();
            await connection.UpdateAsync(item);
            OnItemUpdated?.Invoke(this, item);
        }


        public async Task AddOrUpdate(TodoItem item) {
            if (item.Id == 0) {
                await AddItem(item);
            }
            else {
                await UpdateItem(item);
            }
        }

        public async Task<List<TodoItem>> GetItems() {
            await CreateConnection();
            return await connection.Table<TodoItem>().ToListAsync();
        }

        private async Task CreateConnection() {
            if (connection != null) {
                return;
            }

            var documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var databasePath = Path.Combine(documentPath, "TodoItems.db");

            connection = new SQLiteAsyncConnection(databasePath);
            await connection.CreateTableAsync<TodoItem>();
        }
    }
}
