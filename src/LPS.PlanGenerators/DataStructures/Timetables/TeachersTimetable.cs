using LPS.PlanGenerators.Models;
using LPS.PlanGenerators.ValueObjects;

namespace LPS.PlanGenerators.DataStructures.Timetables;

public class TeachersTimetable : ScheduleTimetableDict<int>
{
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
