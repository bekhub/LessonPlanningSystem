using Avalonia.Controls;
using LPS.Client.Models;
using LPS.Client.ViewModels;

namespace LPS.Client.Views;

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
