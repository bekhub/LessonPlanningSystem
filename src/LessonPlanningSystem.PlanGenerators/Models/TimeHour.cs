namespace LessonPlanningSystem.PlanGenerators.Models;

public class TimeHour
{
    public TimeHour()
    {
        TimeTables = new HashSet<Timetable>();
    }

    public int Id { get; set; }
    public int? OrderPosition { get; set; }
    public string Label { get; set; }

    public ICollection<Timetable> TimeTables { get; set; }
}
