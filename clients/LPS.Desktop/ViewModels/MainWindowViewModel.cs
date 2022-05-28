using System;
using System.Reactive.Linq;
using LPS.Desktop.Models;
using ReactiveUI;

namespace LPS.Desktop.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ViewModelBase _content;

    public TodoListViewModel List { get; }

    public MainWindowViewModel(Database db)
    {
        Content = List = new TodoListViewModel(db.GetItems());
    }

    public ViewModelBase Content {
        get => _content;
        private set => this.RaiseAndSetIfChanged(ref _content, value);
    }

    public void AddItem()
    {
        var vm = new AddItemViewModel();
        Observable.Merge(vm.Ok, vm.Cancel.Select(_ => (TodoItem)null)).Take(1).Subscribe(model => {
            if (model != null) List.Items.Add(model);
            Content = List;
        });
        Content = vm;
    }
}
