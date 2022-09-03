using LPS.PlanGenerators.Enums;

namespace LPS.PlanGenerators.Models;

public class CourseVsRoom : Model
{
    public LessonType LessonType { get; init; }

    public Classroom Classroom { get; init; }
    public Course Course { get; init; }
}
