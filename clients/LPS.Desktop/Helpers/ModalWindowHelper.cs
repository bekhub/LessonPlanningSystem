using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using LPS.Desktop.ViewModels;

namespace LPS.Desktop.Helpers;

public class ModalWindowHelper
{
    public static async Task<TU> ShowModalWindow<T, TU>(ViewModelBase viewModel) where T : Window, new() where TU : ViewModelBase
    {
        var mainWindow = (Avalonia.Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)!
            .Windows[0];

        T window = new T { DataContext = viewModel };

        await window.ShowDialog(mainWindow);
        return (window.DataContext as TU)!;
    }
}
