using Avalonia.Controls;
using LPS.Desktop.Models;
using LPS.Desktop.ViewModels;

namespace LPS.Desktop.Views;

public sealed partial class ConnectionPageView : BaseControl<ConnectionPageViewModel>
{
    public ConnectionPageView()
    {
        InitializeComponent();
    }
    
    private void LsbSavedConnectionDetails_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        ListBox box = (sender as ListBox)!;
        var selectedItem = box.SelectedItem as ConnectionDetails;
        ViewModel!.ConnectionDetailsSelectionChanged(selectedItem);
    }
}
