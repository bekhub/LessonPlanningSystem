using System.Reactive;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace LPS.Client.ViewModels;

public class RouterViewModel : ViewModelBase, IScreen
{
    [Reactive] public RoutingState Router { get; set; } = new();
    public ReactiveCommand<Unit, Unit> GoNext { get; set; }
    public ReactiveCommand<Unit, IRoutableViewModel> GoBack => Router.NavigateBack;
    [Reactive] public bool IsGoNextEnabled { get; set; }
    [Reactive] public bool IsGoBackEnabled { get; set; }
}
