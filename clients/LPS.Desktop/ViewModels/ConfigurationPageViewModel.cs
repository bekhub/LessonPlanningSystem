using System;
using System.Reactive.Disposables;
using LPS.PlanGenerators.Configuration;
using LPS.PlanGenerators.Enums;
using LPS.PlanGenerators.ValueObjects;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace LPS.Desktop.ViewModels;

public class ConfigurationPageViewModel : RoutableViewModel
{
    [Reactive] public bool RemoteCoursesChecked { get; set; }
    [Reactive] public int? SelectedRemoteEducationClassroomId { get; set; }
    [Reactive] public int? SelectedRemoteEducationWeekday { get; set; }
    [Reactive] public int? SelectedRemoteEducationHour { get; set; }
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
        this.WhenActivated(disposable => {
            this.WhenAnyValue(
                    x => x.AutumnEnabled, 
                    x => x.SpringEnabled,
                    x => x.RemoteCoursesChecked,
                    x => x.SelectedRemoteEducationClassroomId,
                    x => x.SelectedRemoteEducationWeekday,
                    x => x.SelectedRemoteEducationHour,
                    (autumn, spring, remote, classroom, weekday, hour) 
                        => (autumn || spring) && !remote || classroom is > 0 && weekday != null && hour != null)
                .Subscribe(x => RouterViewModel.IsGoNextEnabled = x).DisposeWith(disposable);
        });
    }

    public override void OnGoNext()
    {
        ScheduleTime? time = RemoteCoursesChecked 
            ? new ScheduleTime((Weekdays)(SelectedRemoteEducationWeekday! + 1), SelectedRemoteEducationHour!.Value)
            : null;
        RouterViewModel.ConfigurationDetails.PlanConfiguration = new PlanConfiguration {
            IncludeGeneralMandatoryCourses = false,
            IncludeRemoteEducationCourses = RemoteCoursesChecked,
            RemoteEducationLessonTime = time,
            RemoteEducationClassroomId = RemoteCoursesChecked ? SelectedRemoteEducationClassroomId : null,
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
}
