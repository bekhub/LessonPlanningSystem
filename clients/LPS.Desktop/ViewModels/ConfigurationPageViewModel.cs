using System;
using System.Reactive.Disposables;
using LPS.PlanGenerators.Configuration;
using LPS.PlanGenerators.Enums;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace LPS.Desktop.ViewModels;

public class ConfigurationPageViewModel : RoutableViewModel
{
    [Reactive] public bool GeneralMandatoryCoursesChecked { get; set; }
    [Reactive] public bool RemoteCoursesChecked { get; set; }
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
            this.WhenAnyValue(x => x.AutumnEnabled, x => x.SpringEnabled,
                    (autumn, spring) => autumn || spring)
                .Subscribe(x => RouterViewModel.IsGoNextEnabled = x).DisposeWith(disposable);
        });
    }

    public override void OnGoNext()
    {
        RouterViewModel.ConfigurationDetails.PlanConfiguration = new PlanConfiguration {
            IncludeGeneralMandatoryCourses = GeneralMandatoryCoursesChecked,
            IncludeRemoteEducationCourses = RemoteCoursesChecked,
            Semester = AutumnEnabled ? Semester.Autumn : Semester.Spring,
            EducationalYear = $"{EducationYearFrom.Year}-{EducationYearTo.Year}",
            NumberOfVariants = NumberOfTries,
            UnpositionedLessonsCoefficient = UnpositionedLessonsCoefficient,
            SeparatedLessonsCoefficient = SeparatedLessonsCoefficient,
            MaxTeachingHoursCoefficient = MaxTeachingCoefficient,
            MaxNumberOfThreads = NumberOfThreads
        };
        // AutumnEnabled = false;
        // SpringEnabled = false;
        base.OnGoNext();
    }
}
