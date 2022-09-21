#nullable enable
using LPS.PlanGenerators.Enums;
using LPS.PlanGenerators.ValueObjects;

namespace LPS.PlanGenerators.Configuration;

public record PlanConfiguration
{
    public bool IncludeGeneralMandatoryCourses { get; init; }
    public bool IncludeRemoteEducationCourses { get; init; }
    public ScheduleTime? RemoteEducationLessonTime { get; init; }
    public int? RemoteEducationClassroomId { get; init; }
    public Semester Semester { get; init; }
    public string EducationalYear { get; set; } = null!;
    public int NumberOfVariants { get; init; }
    public int UnpositionedLessonsCoefficient { get; init; }
    public int SeparatedLessonsCoefficient { get; init; }
    public int MaxTeachingHoursCoefficient { get; init; }
    public int? MaxNumberOfThreads { get; init; }
}
