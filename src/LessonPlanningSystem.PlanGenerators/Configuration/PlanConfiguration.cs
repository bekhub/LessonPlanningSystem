using LessonPlanningSystem.PlanGenerators.Enums;

namespace LessonPlanningSystem.PlanGenerators.Configuration;

public class PlanConfiguration
{
    public bool IncludeGeneralMandatoryCourses { get; set; }
    public bool IncludeRemoteEducationCourses { get; set; }
    public Semester Semester { get; set; }
    public int NumberOfVariants { get; set; }
    public int UnpositionedLessonsCoefficient { get; set; }
    public int SeparatedLessonsCoefficient { get; set; }
    public int MaxTeachingHoursCoefficient { get; set; }
    public int? MaxNumberOfThreads { get; set; }
}
