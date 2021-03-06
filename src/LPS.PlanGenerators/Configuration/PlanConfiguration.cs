using LPS.PlanGenerators.Enums;

namespace LPS.PlanGenerators.Configuration;

public class PlanConfiguration
{
    public bool IncludeGeneralMandatoryCourses { get; init; }
    public bool IncludeRemoteEducationCourses { get; init; }
    public Semester Semester { get; init; }
    public string EducationalYear { get; set; }
    public int NumberOfVariants { get; init; }
    public int UnpositionedLessonsCoefficient { get; init; }
    public int SeparatedLessonsCoefficient { get; init; }
    public int MaxTeachingHoursCoefficient { get; init; }
    public int? MaxNumberOfThreads { get; init; }
}
