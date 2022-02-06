namespace LessonPlanningSystem.DatabaseLayer.Entities;

public class AcademicDegree
{
    public AcademicDegree()
    {
        Courses = new HashSet<Course>();
    }

    public int Id { get; set; }
    public string Name { get; set; }

    public ICollection<Course> Courses { get; set; }
}