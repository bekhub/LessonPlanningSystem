using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.Selection;
using LPS.DatabaseLayer.Entities;
using LPS.Desktop.Helpers;
using LPS.Desktop.Services;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace LPS.Desktop.ViewModels;

public sealed class SelectItemsPageViewModel : RoutableViewModel
{
    [Reactive] public List<Faculty> Faculties { get; set; }
    [Reactive] public bool AllFacultiesChecked { get; set; }
    public SelectionModel<Faculty> SelectionFaculties { get; set; }
    [Reactive] public List<Department> Departments { get; set; } = new();
    [Reactive] public List<Department> SourceDepartments { get; set; }
    [Reactive] public List<Department> SelectedDepartments { get; set; }
    [Reactive] public bool AllDepartmentsChecked { get; set; }
    public SelectionModel<Department> SelectionDepartments { get; set; }

    [ObservableAsProperty] public bool IsLoading => false;

    public ReactiveCommand<Unit, Unit> AllFacultiesCheckbox { get; set; }
    public ReactiveCommand<Unit, Unit> AllDepartmentsCheckbox { get; set; }

    public SelectItemsPageViewModel(RouterViewModel routerViewModel) : base(routerViewModel, "selectItems")
    {
        SelectionFaculties = new SelectionModel<Faculty> { SingleSelect = false };
        SelectionFaculties.SelectionChanged += AllFacultiesSelectionChanged;
        SelectionDepartments = new SelectionModel<Department> { SingleSelect = false };
        SelectionDepartments.SelectionChanged += AllDepartmentsSelectionChanged;
        Observable.Start(RetrieveDataAsync, RxApp.TaskpoolScheduler);
        this.WhenAny(x => x.Faculties, x => x.SourceDepartments,
                (faculties, departments) =>
                    faculties.Value == null || departments.Value == null)
            .ToPropertyEx(this, x => x.IsLoading);
        AllFacultiesCheckbox = ReactiveCommand.Create(HandleAllFacultiesCheckbox);
        AllDepartmentsCheckbox = ReactiveCommand.Create(HandleAllDepartmentsCheckbox);
        this.WhenActivated(disposable => {
            RouterViewModel.IsGoBackEnabled = true;
            this.WhenAny(x => x.SelectedDepartments,
                items => items.Value != null && items.Value.Count != 0)
                .Subscribe(x => RouterViewModel.IsGoNextEnabled = x).DisposeWith(disposable);
        });
    }

    public override void OnGoNext()
    {
        RouterViewModel.ConfigurationDetails.Departments = SelectedDepartments;
        base.OnGoNext();
    }

    private void HandleAllFacultiesCheckbox()
    {
        if (AllFacultiesChecked) SelectionFaculties.SelectAll();
        else SelectionFaculties.Clear();
    }
    
    private void HandleAllDepartmentsCheckbox()
    {
        if (AllDepartmentsChecked) SelectionDepartments.SelectAll();
        else SelectionDepartments.Clear();
    }
    
    private void AllFacultiesSelectionChanged(object sender, SelectionModelSelectionChangedEventArgs e)
    {
        var selectedFaculties = SelectionFaculties.SelectedItems.Select(x => x.Id);
        Departments = SourceDepartments.Where(x => selectedFaculties.Contains(x.FacultyId!.Value)).ToList();
        AllFacultiesChecked = SelectionFaculties.SelectedItems.Count == Faculties!.Count;
    }
    
    private void AllDepartmentsSelectionChanged(object sender, SelectionModelSelectionChangedEventArgs e)
    {
        if (Departments.Count == 0) AllDepartmentsChecked = false;
        else AllDepartmentsChecked = SelectionDepartments.SelectedItems.Count == Departments.Count;
        SelectedDepartments = SelectionDepartments.SelectedItems.ToList();
    }
    
    private async Task RetrieveDataAsync()
    {
        try {
            await DatabaseService.UsingContextAsync(ConfigurationDetails.ConnectionDetails, async context => {
                Faculties = await context.Faculties.AsNoTracking().ToListAsync();
                SourceDepartments = await context.Departments.AsNoTracking().ToListAsync();
            });
        } catch (Exception ex) {
            Observable.Start(() => MessageBoxHelper.ShowErrorAsync(ex.Message), RxApp.MainThreadScheduler);
        }
    }
}
