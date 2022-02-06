namespace LessonPlanningSystem.DatabaseLayer.Entities;

public class Teacher
{
    public Teacher()
    {
        Courses = new HashSet<Course>();
    }

    public int Id { get; set; }
    public int Code { get; set; }
    public string Name { get; set; }
    public string Title { get; set; }
    public string ExtraData { get; set; }
    public int EmployeeType { get; set; }
    public bool Archived { get; set; }
    public int? UserId { get; set; }

    public FosUser User { get; set; }
    public ICollection<Course> Courses { get; set; }
}
