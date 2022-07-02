#nullable enable
using System;
using System.Reactive;
using LPS.Desktop.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace LPS.Desktop.ViewModels;

public class RouterViewModel : ViewModelBase, IScreen
{
    [Reactive] public RoutingState Router { get; set; } = new();
    [Reactive] public RoutableViewModel? CurrentViewModel { get; set; }
    protected ReactiveCommand<Unit, Unit> GoNext { get; set; }
    protected ReactiveCommand<Unit, Unit> GoBack { get; set; }

    [Reactive] public bool IsGoNextEnabled { get; set; }
    [Reactive] public bool IsGoBackEnabled { get; set; }
    public ConfigurationDetails ConfigurationDetails { get; set; } = new();

    public RouterViewModel()
    {
        GoBack = ReactiveCommand.Create(NavigateBack);
    }

    protected void NavigateTo(RoutableViewModel viewModel)
    {
        CurrentViewModel = viewModel;
        Router.Navigate.Execute(viewModel);
    }

    protected void NavigateBack()
    {
        Router.NavigateBack.Execute()
            .Subscribe(x => CurrentViewModel = x as RoutableViewModel);
    }
}
