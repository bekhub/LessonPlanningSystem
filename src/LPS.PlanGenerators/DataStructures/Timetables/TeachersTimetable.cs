using LPS.PlanGenerators.Models;
using LPS.PlanGenerators.ValueObjects;

namespace LPS.PlanGenerators.DataStructures.Timetables;

public sealed class TeachersTimetable : ScheduleTimetableDict<int>
{
    /// <summary>
    /// This function calculates the maximum number of hours to teach for a teacher without break during one day.
    /// </summary>
    public int MaxTeachingHours()
    {
        if (Count == 0) return 0;
        return Values.Select(x => x.Keys)
            .Max(ScheduleTime.CountMaxContinuousDurationPerDay);
    }
    
    /// <summary>
    /// Checks if the teacher is free at the given time.
    /// </summary>
    public bool TeacherIsFree(Teacher teacher, ScheduleTime time)
    {
        if (!ContainsKey(teacher.Code)) return true;
        return !this[teacher.Code].ContainsKey(time);
    }
    
    /// <summary>
    /// Checks if the teacher is free at the given time.
    /// </summary>
    public bool TeacherIsFree(Teacher teacher, ScheduleTimeRange timeRange)
    {
        return timeRange.GetScheduleTimes().All(x => TeacherIsFree(teacher, x));
    }
}
