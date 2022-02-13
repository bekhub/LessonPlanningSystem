using LessonPlanningSystem.PlanGenerators.Enums;

namespace LessonPlanningSystem.PlanGenerators.Models;

public class CourseVsRoom
{
    public int Id { get; set; }
    public LessonType LessonType { get; set; }
    public bool Archived { get; set; }
    public int ClassroomId { get; set; }
    public int CourseId { get; set; }

    public Classroom Classroom { get; set; }
    public Course Course { get; set; }
}
