using System;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace LPS.Client.ViewModels;

public class ConfigurationPageViewModel : RoutableViewModel
{
    [Reactive] public bool GeneralMandatoryCoursesChecked { get; set; }
    [Reactive] public bool RemoteCoursesChecked { get; set; }
    [Reactive] public bool AutumnEnabled { get; set; }
    [Reactive] public bool SpringEnabled { get; set; }
    [Reactive] public int NumberOfTries { get; set; } = 1;
    [Reactive] public int UnpositionedLessonsCoefficient { get; set; }
    [Reactive] public int SeparatedLessonsCoefficient { get; set; }
    [Reactive] public int MaxTeachingCoefficient { get; set; }
    [Reactive] public int NumberOfThreads { get; set; } = 1;
    public static int MaxNumberOfThreads => Environment.ProcessorCount;
    [Reactive] public DateTimeOffset EducationYearFrom { get; set; } = DateTimeOffset.Now;
    [Reactive] public DateTimeOffset EducationYearTo { get; set; }

    public ConfigurationPageViewModel(RouterViewModel routerViewModel) : base(routerViewModel, "configuration")
    {
        this.WhenAnyValue(x => x.EducationYearFrom)
            .Subscribe(x => EducationYearTo = EducationYearFrom.AddYears(1));
    }
}
