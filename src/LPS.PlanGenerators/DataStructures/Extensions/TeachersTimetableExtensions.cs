using LPS.PlanGenerators.DataStructures.Timetables;
using LPS.PlanGenerators.Models;
using LPS.PlanGenerators.ValueObjects;

namespace LPS.PlanGenerators.DataStructures.Extensions;

public static class TeachersTimetableExtensions
{
    /// <summary>
    /// This function calculates the maximum number of hours to teach for a teacher without break during one day.
    /// </summary>
    public static int MaxTeachingHours(this (TeachersTimetable, TeachersTimetable) teachersTimetable)
    {
        var (first, second) = teachersTimetable;
        return Math.Max(first.MaxTeachingHours(), second.MaxTeachingHours());
    }
    
    /// <summary>
    /// Checks if the teacher is free at the given time.
    /// </summary>
    public static bool TeacherIsFree(this (TeachersTimetable, TeachersTimetable) teachersTimetable, Teacher teacher, 
        ScheduleTime time)
    {
        var (first, second) = teachersTimetable;
        return first.TeacherIsFree(teacher, time) && second.TeacherIsFree(teacher, time);
    }
    
    /// <summary>
    /// Checks if the teacher is free at the given time.
    /// </summary>
    public static bool TeacherIsFree(this (TeachersTimetable, TeachersTimetable) teachersTimetable, Teacher teacher, 
        ScheduleTimeRange timeRange)
    {
        var (first, second) = teachersTimetable;
        return first.TeacherIsFree(teacher, timeRange) && second.TeacherIsFree(teacher, timeRange);
    }
}
