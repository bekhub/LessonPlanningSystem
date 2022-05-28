using LessonPlanningSystem.PlanGenerators.ValueObjects;
using LessonPlanningSystem.PlanGenerators.Enums;

namespace LessonPlanningSystem.PlanGenerators.Models;

public class Timetable
{
    public int? Id { get; init; }
    public Course Course { get; init; }
    public LessonType LessonType { get; init; }
    public ScheduleTime ScheduleTime { get; init; }
    public Classroom Classroom { get; set; }

    public Timetable(Course course, LessonType lessonType, ScheduleTime scheduleTime, Classroom classroom)
    {
        Course = course;
        LessonType = lessonType;
        ScheduleTime = scheduleTime;
        Classroom = classroom;
    }

    public override string ToString()
    {
        return $"[{ScheduleTime}, ({Classroom})]";
    }
}
