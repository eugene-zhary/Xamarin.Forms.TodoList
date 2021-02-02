using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinProjects.Models;
using XamarinProjects.Repositories;
using XamarinProjects.Views;

namespace XamarinProjects.ViewModels
{
    public class MainViewModel : ViewModel
    {
        public bool ShowAll { get; set; }
        public string FilterText => ShowAll ? "All" : "Active";
        private readonly TodoItemRepository repository;
        public ObservableCollection<TodoItemViewModel> Items { get; set; }

        public TodoItemViewModel SelectedItem {
            get { return null; }
            set {
                Device.BeginInvokeOnMainThread(async () => await NavigateToItem(value));
                RaisePropertyChanged(nameof(SelectedItem));
            }
        }
        //выбранный элемент открывается в новом окне
        private async Task NavigateToItem(TodoItemViewModel item) {
            if (item == null) {
                return;
            }

            ItemView itemView = Resolver.Resolve<ItemView>();
            ItemViewModel vm = itemView.BindingContext as ItemViewModel;
            vm.Item = item.Item;

            await Navigation.PushAsync(itemView);
        }


        public MainViewModel(TodoItemRepository repository) {
            repository.OnItemAdded += (sender, item) => Items.Add(CreateTodoItemViewModel(item));
            repository.OnItemUpdated += (sender, item) => Task.Run(async () => await LoadData());
            repository.OnItemRemoved += (sender, item) => Task.Run(async () => await LoadData());

            this.repository = repository;
            Task.Run(async () => await LoadData());
        }


        //загражает данные из базы данных
        private async Task LoadData() {
            var items = await repository.GetItems();

            if (!ShowAll) {
                items = items.Where(x => x.Completed == false).ToList();
            }

            var itemViewModels = items.Select(i => CreateTodoItemViewModel(i));
            Items = new ObservableCollection<TodoItemViewModel>(itemViewModels);
        }



        private TodoItemViewModel CreateTodoItemViewModel(TodoItem item) {
            var itemViewModel = new TodoItemViewModel(item);
            //подпись на события элемента
            itemViewModel.ItemStatusChanged += ItemStatusChanged;
            itemViewModel.ItemRemoved += ItemRemoved;
            return itemViewModel;
        }

        //удаляет элемент из базы данных
        private void ItemRemoved(object sender, EventArgs e) {
            if (sender is TodoItemViewModel item) {
                Task.Run(async () => await repository.RemoveItem(item.Item));
            }
        }

        //обновляет элемент
        private void ItemStatusChanged(object sender, EventArgs e) {
            if (sender is TodoItemViewModel item) {
                if (!ShowAll && item.Item.Completed) {
                    Items.Remove(item);
                }

                Task.Run(async () => await repository.UpdateItem(item.Item));
            }
        }


        public ICommand AddItem => new Command(async () => {
            var itemView = Resolver.Resolve<ItemView>();
            await Navigation.PushAsync(itemView);
        });

        public ICommand ToggleFilter => new Command(async () => {
            ShowAll = !ShowAll;
            await LoadData();
        });
    }
}
