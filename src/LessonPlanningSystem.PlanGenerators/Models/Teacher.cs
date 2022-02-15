using LessonPlanningSystem.PlanGenerators.Enums;

namespace LessonPlanningSystem.PlanGenerators.Models;

public class Teacher
{
    /// <summary>
    /// Teacher's registration number
    /// </summary>
    public int Code { get; set; }
    public TeacherType TeacherType { get; set; }
    
    public bool IsFree() => false;

    public int GetTeacherTime(int hour)
    {
        throw new NotImplementedException();
    }

    public void SetTeacherTime(int hour, int courseId)
    {
        throw new NotImplementedException();
    }
}
