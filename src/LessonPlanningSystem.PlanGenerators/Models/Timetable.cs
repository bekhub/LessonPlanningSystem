using LessonPlanningSystem.PlanGenerators.ValueObjects;
using LessonPlanningSystem.PlanGenerators.Enums;

namespace LessonPlanningSystem.PlanGenerators.Models;

public class Timetable
{
    public LessonType LessonType { get; set; }
    public ScheduleTime ScheduleTime { get; set; }
    // Todo: there is a problem with one classroom for one timetable. There are situations where there may be two classrooms for the same timetable
    public Classroom Classroom { get; set; }
    public List<Classroom> Classrooms { get; set; }
    public Course Course { get; set; }
}
