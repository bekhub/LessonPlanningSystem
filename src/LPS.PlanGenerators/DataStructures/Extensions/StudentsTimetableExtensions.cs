using LPS.PlanGenerators.DataStructures.Timetables;
using LPS.PlanGenerators.Enums;
using LPS.PlanGenerators.Models;
using LPS.PlanGenerators.ValueObjects;

namespace LPS.PlanGenerators.DataStructures.Extensions;

public static class StudentsTimetableExtensions
{
    /// <summary>
    /// Checks if students of the given year level and department are free at the given time
    /// </summary>
    public static bool StudentsAreFree(this (StudentsTimetable, StudentsTimetable) studentsTimetable, Course course, 
        ScheduleTime time, Round round)
    {
        var (first, second) = studentsTimetable;
        return first.StudentsAreFree(course, time, round) && second.StudentsAreFree(course, time, round);
    }
    
    /// <summary>
    /// Checks if students of the given year level and department are free at the given time
    /// </summary>
    public static bool StudentsAreFree(this (StudentsTimetable, StudentsTimetable) studentsTimetable, Course course, 
        ScheduleTimeRange timeRange, Round round)
    {
        return timeRange.GetScheduleTimes().All(time => studentsTimetable.StudentsAreFree(course, time, round));
    }
}
