using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XamarinProjects.Models;

namespace XamarinProjects.Repositories
{
    interface ITodoItemRepository
    {
        event EventHandler<TodoItem> OnItemAdded;
        event EventHandler<TodoItem> OnItemUpdated;
        event EventHandler<TodoItem> OnItemRemoved;

        Task<List<TodoItem>> GetItems();
        Task AddItem(TodoItem item);
        Task RemoveItem(TodoItem item);
        Task UpdateItem(TodoItem item);
        Task AddOrUpdate(TodoItem item);
    }
}
