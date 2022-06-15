using System;
using System.Linq;
using ReactiveUI;

namespace LPS.Client.ViewModels;

public class MainViewModel : RouterViewModel
{
    public MainViewModel()
    {
        GoNext = ReactiveCommand.Create(HandleGoNext);
        IsGoNextEnabled = true;
    }

    private void HandleGoNext()
    {
        if (Router.NavigationStack.Count == 0) {
            Router.Navigate.Execute(new ConnectionPageViewModel(this));
            return;
        }
        RoutableViewModel next = Router.NavigationStack.Last().UrlPathSegment switch {
            "connection" => new SelectItemsPageViewModel(this),
            "selectItems" => new ConfigurationPageViewModel(this),
            "configuration" => throw new NotImplementedException(),
            _ => throw new ArgumentOutOfRangeException()
        };
        Router.Navigate.Execute(next);
    }
}
