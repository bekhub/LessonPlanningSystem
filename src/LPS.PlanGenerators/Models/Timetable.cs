#nullable enable
using LPS.PlanGenerators.Enums;
using LPS.PlanGenerators.ValueObjects;

namespace LPS.PlanGenerators.Models;

public sealed class Timetable
{
    public int? Id { get; init; }
    public Course Course { get; init; }
    public LessonType LessonType { get; init; }
    public ScheduleTime ScheduleTime { get; init; }
    public Classroom Classroom { get; set; }
    public Classroom? AdditionalClassroom { get; set; }
    
    public Timetable(Course course, LessonType lessonType, ScheduleTime scheduleTime, Classroom classroom, 
        Classroom? additionalClassroom = null)
    {
        Course = course;
        LessonType = lessonType;
        ScheduleTime = scheduleTime;
        Classroom = classroom;
        AdditionalClassroom = additionalClassroom;
    }
    
    public Timetable(int id, Course course, LessonType lessonType, ScheduleTime scheduleTime, Classroom classroom, 
        Classroom? additionalClassroom = null) : this(course, lessonType, scheduleTime, classroom, additionalClassroom)
    {
        Id = id;
    }

    public override string ToString()
    {
        return $"[{ScheduleTime}, ({Classroom})]";
    }
}
