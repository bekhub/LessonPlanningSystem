using Avalonia.Markup.Xaml;
using LPS.Client.ViewModels;

namespace LPS.Client.Views;

public partial class SelectItemsPageView : BaseControl<SelectItemsPageViewModel>
{
    public SelectItemsPageView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}

