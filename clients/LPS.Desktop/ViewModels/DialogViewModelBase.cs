namespace LPS.Desktop.ViewModels;

public sealed class DialogViewModelBase : ViewModelBase
{
    public bool Cancel { get; set; }

    public DialogViewModelBase()
    {
        Cancel = true;
    }
}
