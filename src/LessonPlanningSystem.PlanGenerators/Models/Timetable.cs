using LessonPlanningSystem.PlanGenerators.ValueObjects;
using LessonPlanningSystem.PlanGenerators.Enums;

namespace LessonPlanningSystem.PlanGenerators.Models;

public class Timetable
{
    public LessonType LessonType { get; set; }
    public ScheduleTime ScheduleTime { get; set; }
    public List<Classroom> Classrooms { get; set; }
    public Course Course { get; set; }
}
