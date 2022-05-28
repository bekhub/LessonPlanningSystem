using LessonPlanningSystem.PlanGenerators.Models;
using LessonPlanningSystem.PlanGenerators.ValueObjects;

namespace LessonPlanningSystem.PlanGenerators.DataStructures.Timetables;

public class ClassroomsTimetable : ScheduleTimetablesDict<int>
{
    /// <summary>
    /// Checks if the room is free at that time
    /// </summary>
    /// <param name="classroom"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public bool RoomIsFree(Classroom classroom, ScheduleTime time)
    {
        if (!ContainsKey(classroom.Id)) return true;
        return !this[classroom.Id].ContainsKey(time);
    }
    
    /// <summary>
    /// Checks if the room is free at that time range
    /// </summary>
    /// <param name="classroom"></param>
    /// <param name="timeRange"></param>
    /// <returns></returns>
    public bool RoomIsFree(Classroom classroom, ScheduleTimeRange timeRange)
    {
        return timeRange.GetScheduleTimes().All(x => RoomIsFree(classroom, x));
    }
}
