namespace LessonPlanningSystem.DatabaseLayer.Entities;

public class LessonType
{
    public LessonType()
    {
        Courses = new HashSet<Course>();
        TimeTables = new HashSet<TimeTable>();
    }

    public int Id { get; set; }
    public string Name { get; set; }

    public ICollection<Course> Courses { get; set; }
    public ICollection<TimeTable> TimeTables { get; set; }
}