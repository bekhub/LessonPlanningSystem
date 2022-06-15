#nullable enable
using ReactiveUI;

namespace LPS.Client.ViewModels;

public abstract class RoutableViewModel : ViewModelBase, IRoutableViewModel
{
    protected RouterViewModel RouterViewModel { get; }
    public IScreen HostScreen { get; }
    public string UrlPathSegment { get; }
    
    public RoutableViewModel(RouterViewModel routerViewModel, string urlPath)
    {
        UrlPathSegment = urlPath;
        HostScreen = routerViewModel;
        RouterViewModel = routerViewModel;
    }

    public virtual void OnSwitchedToThis() { }
}
