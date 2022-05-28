using System.Collections.Generic;
using System.Collections.ObjectModel;
using LPS.Desktop.Models;

namespace LPS.Desktop.ViewModels;

public class TodoListViewModel : ViewModelBase
{
    public ObservableCollection<TodoItem> Items { get; private set; }

    public TodoListViewModel(IEnumerable<TodoItem> items)
    {
        Items = new ObservableCollection<TodoItem>(items);
    }
}
