using LessonPlanningSystem.PlanGenerators.ValueObjects;
using LessonPlanningSystem.PlanGenerators.Enums;

namespace LessonPlanningSystem.PlanGenerators.Models;

public class Timetable
{
    public int? Id { get; init; }
    public LessonType LessonType { get; init; }
    public ScheduleTime ScheduleTime { get; init; }
    public List<Classroom> Classrooms { get; init; }
    public Course Course { get; init; }
}
