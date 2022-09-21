using System;
using System.Reactive.Linq;
using Avalonia;
using FluentAvalonia.Styling;
using LPS.Desktop.Helpers;
using ReactiveUI;

namespace LPS.Desktop.ViewModels;

public class MainViewModel : RouterViewModel
{
    public MainViewModel()
    {
        var faTheme = AvaloniaLocator.Current.GetService<FluentAvaloniaTheme>();
        faTheme!.RequestedTheme = "Dark";
        this.NavigateTo(new ConnectionPageViewModel(this));
        GoNext = ReactiveCommand.Create(HandleGoNext);
    }

    private void HandleGoNext()
    {
        try {
            CurrentViewModel?.OnGoNext();
            if (CurrentViewModel == null) {
                NavigateTo(new ConnectionPageViewModel(this));
                return;
            }
            RoutableViewModel next = CurrentViewModel switch {
                ConnectionPageViewModel => new SelectItemsPageViewModel(this),
                SelectItemsPageViewModel => new RemoteCoursesPageViewModel(this),
                RemoteCoursesPageViewModel => new ConfigurationPageViewModel(this),
                ConfigurationPageViewModel => new TimetableGeneratorViewModel(this),
                _ => throw new ArgumentOutOfRangeException()
            };
            this.NavigateTo(next);
        } catch (Exception ex) {
            Observable.Start(() => MessageBoxHelper.ShowErrorAsync(ex.Message), RxApp.MainThreadScheduler);
        }
    }
}
