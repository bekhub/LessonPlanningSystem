#nullable enable
using System.Reactive.Disposables;
using LPS.Desktop.Models;
using ReactiveUI;

namespace LPS.Desktop.ViewModels;

public abstract class RoutableViewModel : ViewModelBase, IRoutableViewModel
{
    protected RouterViewModel RouterViewModel { get; }
    public IScreen HostScreen { get; }
    public string UrlPathSegment { get; }
    public ConfigurationDetails ConfigurationDetails => RouterViewModel.ConfigurationDetails;
    
    public RoutableViewModel(RouterViewModel routerViewModel, string urlPath)
    {
        UrlPathSegment = urlPath;
        HostScreen = routerViewModel;
        RouterViewModel = routerViewModel;
        this.WhenActivated(disposable => {
            this.HandleActivation();
            Disposable.Create(HandleDeactivation).DisposeWith(disposable);
        });
    }
    
    protected virtual void HandleActivation() { }
    
    protected virtual void HandleDeactivation() { }

    public virtual void OnGoNext() { }
}
