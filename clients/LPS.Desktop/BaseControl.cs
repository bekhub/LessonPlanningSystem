using Avalonia.ReactiveUI;
using ReactiveUI;

namespace LPS.Desktop;

public class BaseControl<TViewModel>: ReactiveUserControl<TViewModel> where TViewModel: class
{
    public BaseControl(bool activate = true)
    {
        if (activate) {
            this.WhenActivated(_ => { });
        }
    }
}
