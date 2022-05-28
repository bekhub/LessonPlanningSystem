using LessonPlanningSystem.PlanGenerators.Enums;

namespace LessonPlanningSystem.PlanGenerators.Models;

public class Teacher : Entity
{
    public override int Id => Code;

    /// <summary>
    /// Teacher's registration number
    /// </summary>
    public int Code { get; init; }
    public TeacherType TeacherType { get; init; }
}
