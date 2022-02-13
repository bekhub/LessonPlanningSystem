using LessonPlanningSystem.PlanGenerators.Enums;

namespace LessonPlanningSystem.PlanGenerators.Configuration;

public class PlanConfiguration
{
    public bool IncludeGeneralMandatoryCourse { get; set; }
    public bool IncludeRemoteEducationCourse { get; set; }
    public Semester Semester { get; set; }
    public int NumberOfVariants { get; set; }
    public int UnpositionedLessonsCoefficient { get; set; }
    public int SeparatedLessonsCoefficient { get; set; }
    public int MaxTeachingHoursCoefficient { get; set; }
    public int WeekHoursNumber { get; set; } = 60;
    public int HoursPerDay { get; set; } = 12;
    public int LunchAfterHour { get; set; } = 4;
    public int RadiusAroundBuilding { get; set; } = 4;
    public int? MaxNumberOfThreads { get; set; }
}
