namespace LPS.Client.ViewModels;

public class DialogViewModelBase : ViewModelBase
{
    public bool Cancel { get; set; }

    public DialogViewModelBase()
    {
        Cancel = true;
    }
}
