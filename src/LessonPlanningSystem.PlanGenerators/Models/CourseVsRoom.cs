using LessonPlanningSystem.PlanGenerators.Enums;

namespace LessonPlanningSystem.PlanGenerators.Models;

public class CourseVsRoom : Entity
{
    public LessonType LessonType { get; init; }

    public Classroom Classroom { get; init; }
    public Course Course { get; init; }
}
