using LPS.PlanGenerators.Enums;

namespace LPS.PlanGenerators.Models;

public sealed class Teacher : Model
{
    public override int Id => Code;

    /// <summary>
    /// Teacher's registration number
    /// </summary>
    public int Code { get; init; }
    public TeacherType TeacherType { get; init; }
}
