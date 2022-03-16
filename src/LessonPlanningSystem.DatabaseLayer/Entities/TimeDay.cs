namespace LessonPlanningSystem.DatabaseLayer.Entities;

public class TimeDay
{
    public int Id { get; set; }
    public int? OrderPosition { get; set; }
    public string Label { get; set; }

    public ICollection<TimeTable> TimeTables { get; set; }
}
