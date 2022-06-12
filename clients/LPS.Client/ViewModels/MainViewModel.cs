using System.Reactive;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace LPS.Client.ViewModels;

public class MainViewModel : ViewModelBase
{
    // [Reactive] public RoutingState Router { get; } = new();
    // public ReactiveCommand<Unit, IRoutableViewModel> GoNext { get; }
    // public ReactiveCommand<Unit, IRoutableViewModel?> GoBack => Router.NavigateBack;
    public ViewModelBase Content { get; set; }

    public MainViewModel()
    {
        Content = new ConnectionPageViewModel();
        // Router.Navigate.Execute(new ConnectionPageViewModel(this));
    }
}
