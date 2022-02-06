namespace LessonPlanningSystem.DatabaseLayer.Entities;

public class CourseType
{
    public CourseType()
    {
        Courses = new HashSet<Course>();
    }

    public int Id { get; set; }
    public string Type { get; set; }
    public int TypeCode { get; set; }
    public bool Archived { get; set; }

    public ICollection<Course> Courses { get; set; }
}
