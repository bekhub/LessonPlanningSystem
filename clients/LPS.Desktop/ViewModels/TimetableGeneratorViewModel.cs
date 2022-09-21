#nullable enable
using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using LPS.Application;
using LPS.Desktop.Helpers;
using LPS.PlanGenerators;
using LPS.PlanGenerators.DataStructures;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using static LPS.Desktop.Services.DatabaseService;

namespace LPS.Desktop.ViewModels;

public class TimetableGeneratorViewModel : RoutableViewModel
{
    [Reactive] private CoursesData? CoursesData { get; set; }
    [Reactive] private ClassroomsData? ClassroomsData { get; set; }
    [Reactive] private ExistingTimetable? ExistingTimetable { get; set; }
    [Reactive] private GeneratedLessonPlan? GeneratedLessonPlan { get; set; }

    [ObservableAsProperty] public bool IsDataPulling { get; }
    [ObservableAsProperty] public bool IsPlanGenerating { get; }
    [ObservableAsProperty] public bool IsOriginalSaving { get; }
    [ObservableAsProperty] public bool IsPreviewSaving { get; }
    [ObservableAsProperty] public bool IsProgressActive { get; }
    [ObservableAsProperty] public bool CanGenerate { get; }
    [ObservableAsProperty] public bool CanSave { get; }

    [ObservableAsProperty] public int? TotalFreeHoursOfRooms { get; }
    [ObservableAsProperty] public int? TotalUnpositionedLessons { get; }
    [ObservableAsProperty] public int? TotalUnpositionedCourses { get; }
    [ObservableAsProperty] public int? TotalSeparatedLessons { get; }
    [ObservableAsProperty] public int? MaxTeachingHours { get; }

    public ReactiveCommand<Unit, Task> GenerateLessonPlan { get; set; } = null!;
    public ReactiveCommand<Unit, Unit> SaveAsOriginal { get; set; } = null!;
    public ReactiveCommand<Unit, Unit> SaveAsPreview { get; set; } = null!;

    public TimetableGeneratorViewModel(RouterViewModel routerViewModel) :
        base(routerViewModel, "timetableGenerator")
    {
        RouterViewModel.IsGoNextEnabled = false;
        CreateObservables();
        CreateCommands();
        PullData();
        this.WhenActivated(disposable => {
            this.WhenAny(x => x.IsProgressActive,
                    active => active.Value)
                .Subscribe(x => RouterViewModel.IsGoBackEnabled = !x).DisposeWith(disposable);
        });
    }

    private void CreateObservables()
    {
        this.WhenAnyValue(x => x.CoursesData, x => x.ClassroomsData, 
                (courses, rooms) => courses == null || rooms == null)
            .ToPropertyEx(this, x => x.IsDataPulling);
        this.WhenAny(x => x.GeneratedLessonPlan, 
                plan => plan.Value?.TotalFreeHoursOfRooms)
            .ToPropertyEx(this, x => x.TotalFreeHoursOfRooms);
        this.WhenAny(x => x.GeneratedLessonPlan,
                plan => plan.Value?.TotalUnpositionedLessons)
            .ToPropertyEx(this, x => x.TotalUnpositionedLessons);
        this.WhenAny(x => x.GeneratedLessonPlan,
                plan => plan.Value?.TotalSeparatedLessons)
            .ToPropertyEx(this, x => x.TotalSeparatedLessons);
        this.WhenAny(x => x.GeneratedLessonPlan,
                plan => plan.Value?.TotalUnpositionedCourses)
            .ToPropertyEx(this, x => x.TotalUnpositionedCourses);
        this.WhenAny(x => x.GeneratedLessonPlan,
                plan => plan.Value?.MaxTeachingHours)
            .ToPropertyEx(this, x => x.MaxTeachingHours);
        this.WhenAnyValue(x => x.IsPlanGenerating, x => x.IsPreviewSaving,
                x => x.IsOriginalSaving, 
                (generating, pSaving, oSaving) => generating || pSaving || oSaving)
            .ToPropertyEx(this, x => x.IsProgressActive);
        this.WhenAnyValue(x => x.GeneratedLessonPlan, x => x.IsProgressActive, 
                (plan, progressActive) => plan != null && !progressActive)
            .ToPropertyEx(this, x => x.CanSave);
        this.WhenAnyValue(x => x.IsProgressActive, 
                progressActive => !progressActive)
            .ToPropertyEx(this, x => x.CanGenerate);
    }

    private void CreateCommands()
    {
        GenerateLessonPlan = ReactiveCommand.CreateFromObservable(GenerateTimetableImpl);
        GenerateLessonPlan.IsExecuting.ToPropertyEx(this, x => x.IsPlanGenerating);
        SaveAsOriginal = ReactiveCommand.CreateFromTask(SaveAsOriginalAsync);
        SaveAsOriginal.IsExecuting.ToPropertyEx(this, x => x.IsOriginalSaving);
        SaveAsPreview = ReactiveCommand.CreateFromTask(SaveAsPreviewAsync);
        SaveAsPreview.IsExecuting.ToPropertyEx(this, x => x.IsPreviewSaving);
    }

    private IObservable<Task> GenerateTimetableImpl()
    {
        return Observable.Start(GenerateTimetableAsync, RxApp.TaskpoolScheduler);
    }
    
    private void PullData()
    {
        Observable.Start(PullAllDataAsync, RxApp.TaskpoolScheduler).ObserveOn(RxApp.MainThreadScheduler);
    }

    private async Task GenerateTimetableAsync()
    {
        try {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var provider = new GeneratorServiceProvider(ConfigurationDetails.PlanConfiguration, CoursesData,
                ClassroomsData, ExistingTimetable);
            var planGenerator = new BestPlanGenerator(provider);
            GeneratedLessonPlan = await planGenerator.GenerateBestLessonPlanAsync();
            stopwatch.Stop();
            await Observable.Start(
                () => MessageBoxHelper.ShowSuccessAsync("Lesson plan generated in " + stopwatch.Elapsed),
                RxApp.MainThreadScheduler);
        } catch (Exception ex) {
            await Observable.Start(() => MessageBoxHelper.ShowErrorAsync(ex.Message), RxApp.MainThreadScheduler);
        }
    }

    private async Task SaveAsOriginalAsync()
    {
        try {
            await UsingTimetableServiceAsync(ConfigurationDetails, async service => {
                await service.SaveTimetableAsOriginalAsync(GeneratedLessonPlan!);
            });
            await MessageBoxHelper.ShowSuccessAsync("Lesson plan saved to original table");
        } catch (Exception ex) {
            await MessageBoxHelper.ShowErrorAsync(ex.Message);
        }
    }
    
    private async Task SaveAsPreviewAsync()
    {
        try {
            await UsingTimetableServiceAsync(ConfigurationDetails, async service => {
                await service.SaveTimetableAsPreviewAsync(GeneratedLessonPlan!);
            });
            await MessageBoxHelper.ShowSuccessAsync("Lesson plan saved to preview table");
        } catch (Exception ex) {
            await MessageBoxHelper.ShowErrorAsync(ex.Message);
        }
    }
    
    private async Task PullAllDataAsync()
    {
        try {
            await UsingTimetableServiceAsync(ConfigurationDetails, async service => {
                CoursesData = await service.GetCoursesDataAsync(ConfigurationDetails.Departments);
                ClassroomsData = await service.GetClassroomsDataAsync(CoursesData.AllCourseList);
                ExistingTimetable = await service.GetExistingTimetable(CoursesData, ClassroomsData);
            });
        } catch (Exception ex) {
            await Observable.Start(() => MessageBoxHelper.ShowErrorAsync(ex.Message), RxApp.MainThreadScheduler);
        }
    }
}
