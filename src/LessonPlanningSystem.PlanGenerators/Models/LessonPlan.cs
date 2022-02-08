using LessonPlanningSystem.PlanGenerators.Enums;

namespace LessonPlanningSystem.PlanGenerators.Models;

public class LessonPlan
{
    public string EducationalYear { get; set; }
    public Semester Semester { get; set; }
    public int? ClassroomId { get; set; }
    public int CourseId { get; set; }
    public LessonType LessonType { get; set; }

    public Classroom Classroom { get; set; }
    public Course Course { get; set; }
}
