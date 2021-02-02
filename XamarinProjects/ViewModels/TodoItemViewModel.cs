using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinProjects.Models;

namespace XamarinProjects.ViewModels
{
    public class TodoItemViewModel :ViewModel
    {
        public TodoItem Item { get; private set; }
        public string StatusText => Item.Completed ? "Reactivate" : "Completed";

        public TodoItemViewModel(TodoItem item) {
            this.Item = item;
        }

        public event EventHandler ItemStatusChanged;
        public event EventHandler ItemRemoved;

        public ICommand ToggleCompleted => new Command((arg) => {
            Item.Completed = !Item.Completed;
            ItemStatusChanged?.Invoke(this, new EventArgs());
        });
        public ICommand RemoveItem => new Command((arg) => {
            ItemRemoved?.Invoke(this, new EventArgs());
        });
    }
}
