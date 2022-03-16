using LessonPlanningSystem.PlanGenerators.Enums;

namespace LessonPlanningSystem.PlanGenerators.Models;

public class CourseVsRoom
{
    public int Id { get; init; }
    public LessonType LessonType { get; init; }

    public Classroom Classroom { get; init; }
    public Course Course { get; init; }
}
