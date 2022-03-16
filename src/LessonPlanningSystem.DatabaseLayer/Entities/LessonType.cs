namespace LessonPlanningSystem.DatabaseLayer.Entities;

public class LessonType
{
    public int Id { get; set; }
    public string Name { get; set; }

    public ICollection<Course> Courses { get; set; }
    public ICollection<TimeTable> TimeTables { get; set; }
}
