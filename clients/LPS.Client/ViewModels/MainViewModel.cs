using System;
using ReactiveUI;

namespace LPS.Client.ViewModels;

public class MainViewModel : RouterViewModel
{
    public MainViewModel()
    {
        this.NavigateTo(new ConnectionPageViewModel(this));
        GoNext = ReactiveCommand.Create(HandleGoNext);
    }

    private void HandleGoNext()
    {
        CurrentViewModel?.OnGoNext();
        if (CurrentViewModel == null) {
            NavigateTo(new ConnectionPageViewModel(this));
            return;
        }
        RoutableViewModel next = CurrentViewModel switch {
            ConnectionPageViewModel => new SelectItemsPageViewModel(this),
            SelectItemsPageViewModel => new ConfigurationPageViewModel(this),
            ConfigurationPageViewModel => new TimetableGeneratorViewModel(this),
            _ => throw new ArgumentOutOfRangeException()
        };
        this.NavigateTo(next);
    }
}
