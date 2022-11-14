#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using LPS.DatabaseLayer.Entities;
using LPS.Desktop.Helpers;
using LPS.Desktop.Services;
using LPS.PlanGenerators.Configuration;
using LPS.PlanGenerators.Enums;
using LPS.PlanGenerators.ValueObjects;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace LPS.Desktop.ViewModels;

public sealed class RemoteCoursesPageViewModel : RoutableViewModel
{
    [Reactive] public List<TimeHour> TimeHours { get; set; }
    [Reactive] public List<Classroom> Classrooms { get; set; }
    [Reactive] public bool RemoteCoursesChecked { get; set; }
    [Reactive] public int? SelectedWeekday { get; set; } = 0;
    [Reactive] public TimeHour? SelectedTimeHour { get; set; }
    [Reactive] public Classroom? SelectedClassroom { get; set; }

    public RemoteCoursesPageViewModel(RouterViewModel routerViewModel) : base(routerViewModel, "remoteCourses")
    {
        Observable.Start(RetrieveDataAsync, RxApp.TaskpoolScheduler);
        this.WhenActivated(disposable => {
            this.WhenAnyValue(
                    x => x.RemoteCoursesChecked,
                    x => x.SelectedClassroom,
                    x => x.SelectedWeekday,
                    x => x.SelectedTimeHour,
                    (remote, classroom, weekday, hour) 
                        => !remote || classroom != null && weekday != null && hour != null)
                .Subscribe(x => RouterViewModel.IsGoNextEnabled = x).DisposeWith(disposable);
        });
    }

    public override void OnGoNext()
    {
        ScheduleTime? time = RemoteCoursesChecked 
            ? new ScheduleTime((Weekdays)(SelectedWeekday! + 1), SelectedTimeHour!.OrderPosition!.Value)
            : null;
        RouterViewModel.ConfigurationDetails.PlanConfiguration = new PlanConfiguration {
            IncludeGeneralMandatoryCourses = false,
            IncludeRemoteEducationCourses = RemoteCoursesChecked,
            RemoteEducationLessonTime = time,
            RemoteEducationClassroomId = RemoteCoursesChecked ? SelectedClassroom?.Id : null,
        };
        base.OnGoNext();
    }
    
    private async Task RetrieveDataAsync()
    {
        try {
            await DatabaseService.UsingContextAsync(ConfigurationDetails.ConnectionDetails, async context => {
                TimeHours = await context.TimeHours.AsNoTracking().ToListAsync();
                Classrooms = await context.Classrooms
                    .Include(x => x.Building)
                    .Where(x => !x.Archived).AsNoTracking().ToListAsync();
                SelectedTimeHour = TimeHours.FirstOrDefault();
            });
        } catch (Exception ex) {
            Observable.Start(() => MessageBoxHelper.ShowErrorAsync(ex.Message), RxApp.MainThreadScheduler);
        }
    }
}
