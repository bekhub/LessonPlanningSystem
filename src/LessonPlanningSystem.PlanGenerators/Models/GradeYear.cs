namespace LessonPlanningSystem.PlanGenerators.Models;

public class GradeYear
{
    public int Id { get; set; }
    public int Grade { get; set; }
    public bool Archived { get; set; }
    public int? DepartmentId { get; set; }

    public Department Department { get; set; }
}
