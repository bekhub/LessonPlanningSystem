using System.Reactive.Disposables;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace LPS.Client;

public class BaseControl<TViewModel>: ReactiveUserControl<TViewModel> where TViewModel: class
{
    public BaseControl()
    {
        this.WhenActivated(disposables => 
            Disposable.Create(() => { }).DisposeWith(disposables));
    }
    
    public BaseControl(bool activate)
    {
        if (activate) {
            this.WhenActivated(disposables => 
                Disposable.Create(() => { }).DisposeWith(disposables));
        }
    }
}
