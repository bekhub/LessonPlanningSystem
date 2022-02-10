namespace LessonPlanningSystem.PlanGenerators.Models;

public class Department
{
    public Department()
    {
        Classrooms = new HashSet<Classroom>();
        Courses = new HashSet<Course>();
        GradeYears = new HashSet<GradeYear>();
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public bool Archived { get; set; }
    public int? FacultyId { get; set; }

    public Faculty Faculty { get; set; }
    public ICollection<Classroom> Classrooms { get; set; }
    public ICollection<Course> Courses { get; set; }
    public ICollection<GradeYear> GradeYears { get; set; }
}
