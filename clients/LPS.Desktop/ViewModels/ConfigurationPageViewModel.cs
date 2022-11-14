using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using LPS.DatabaseLayer;
using LPS.DatabaseLayer.Entities;
using LPS.Desktop.Helpers;
using LPS.Desktop.Services;
using LPS.PlanGenerators.Configuration;
using LPS.PlanGenerators.Enums;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace LPS.Desktop.ViewModels;

public sealed class ConfigurationPageViewModel : RoutableViewModel
{
    [Reactive] public List<TimeHour> TimeHours { get; set; }
    [Reactive] public TimeHour LunchAfterHour { get; set; }
    [Reactive] public TimeHour HourStart { get; set; }
    [Reactive] public TimeHour HourEnd { get; set; }
    [Reactive] public bool AutumnEnabled { get; set; }
    [Reactive] public bool SpringEnabled { get; set; }
    [Reactive] public int NumberOfTries { get; set; } = 1;
    [Reactive] public int UnpositionedLessonsCoefficient { get; set; } = 100;
    [Reactive] public int SeparatedLessonsCoefficient { get; set; } = 10;
    [Reactive] public int MaxTeachingCoefficient { get; set; } = 1;
    [Reactive] public int NumberOfThreads { get; set; } = MaxNumberOfThreads - 1;
    public static int MaxNumberOfThreads => Environment.ProcessorCount;
    [Reactive] public DateTimeOffset EducationYearFrom { get; set; } = DateTimeOffset.Now;
    [Reactive] public DateTimeOffset EducationYearTo { get; set; }

    public ConfigurationPageViewModel(RouterViewModel routerViewModel) : base(routerViewModel, "configuration")
    {
        RouterViewModel.IsGoBackEnabled = true;
        this.WhenAnyValue(x => x.EducationYearFrom)
            .Subscribe(x => EducationYearTo = x.AddYears(1));
        Observable.Start(RetrieveDataAsync, RxApp.TaskpoolScheduler);
        this.WhenActivated(disposable => {
            this.WhenAnyValue(
                    x => x.AutumnEnabled, 
                    x => x.SpringEnabled,
                    x => x.LunchAfterHour,
                    x => x.HourStart,
                    x => x.HourEnd,
                    (autumn, spring, lunchAfterHour, hourStart, hourEnd) 
                        => (autumn || spring) && lunchAfterHour != null && hourStart != null && hourEnd != null)
                .Subscribe(x => RouterViewModel.IsGoNextEnabled = x).DisposeWith(disposable);
        });
    }

    public override void OnGoNext()
    {
        StaticConfiguration.LunchAfterHour = LunchAfterHour.OrderPosition!.Value;
        StaticConfiguration.HourStart = HourStart.OrderPosition!.Value;
        StaticConfiguration.HourEnd = HourEnd.OrderPosition!.Value;
        RouterViewModel.ConfigurationDetails.PlanConfiguration = RouterViewModel.ConfigurationDetails.PlanConfiguration with {
            Semester = AutumnEnabled ? Semester.Autumn : Semester.Spring,
            EducationalYear = $"{EducationYearFrom.Year}-{EducationYearTo.Year}",
            NumberOfVariants = NumberOfTries,
            UnpositionedLessonsCoefficient = UnpositionedLessonsCoefficient,
            SeparatedLessonsCoefficient = SeparatedLessonsCoefficient,
            MaxTeachingHoursCoefficient = MaxTeachingCoefficient,
            MaxNumberOfThreads = NumberOfThreads
        };
        base.OnGoNext();
    }

    private async Task RetrieveDataAsync()
    {
        try {
            await DatabaseService.UsingContextAsync(ConfigurationDetails.ConnectionDetails, async context => {
                TimeHours = await context.TimeHours.AsNoTracking().ToListAsync();
                await RetrieveConfigAsync(context);
            });
        } catch (Exception ex) {
            Observable.Start(() => MessageBoxHelper.ShowErrorAsync(ex.Message), RxApp.MainThreadScheduler);
        }
    }

    private async Task RetrieveConfigAsync(TimetableContext context)
    {
        var semester = await GetConfigValueAsync(context, "Semester");
        if (semester != null) {
            AutumnEnabled = semester == "Guz";
            SpringEnabled = semester == "Bahar";
        }
        var educationalYear = await GetConfigValueAsync(context, "EducationalYear");
        if (educationalYear != null) {
            var from = educationalYear[..4] + "-01-01";
            EducationYearFrom = DateTimeOffset.Parse(from);
        }
        var lunchAfterHour = await GetConfigValueAsync(context, "LunchAfterHourPosition");
        var hourStart = await GetConfigValueAsync(context, "HourStartPosition");
        var hourEnd = await GetConfigValueAsync(context, "HourEndPosition");
        if (lunchAfterHour != null && hourStart != null && hourEnd != null) {
            LunchAfterHour = TimeHours.FirstOrDefault(x => x.OrderPosition == Convert.ToInt32(lunchAfterHour));
            HourStart = TimeHours.FirstOrDefault(x => x.OrderPosition == Convert.ToInt32(hourStart));
            HourEnd = TimeHours.FirstOrDefault(x => x.OrderPosition == Convert.ToInt32(hourEnd));
        }
    }

    private static async Task<string> GetConfigValueAsync(TimetableContext context, string configName)
    {
        var config = await context.Configs.AsNoTracking().FirstOrDefaultAsync(x => x.Name == configName);
        return config?.Value;
    }
}
