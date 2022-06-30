using LPS.PlanGenerators.DataStructures.Timetables;
using LPS.PlanGenerators.Enums;
using LPS.PlanGenerators.Models;
using LPS.PlanGenerators.ValueObjects;

namespace LPS.PlanGenerators.DataStructures.Extensions;

public static class ClassroomsTimetableExtensions
{
    /// <summary>
    /// Checks if the room is free at that time range
    /// </summary>
    public static bool RoomIsFree(this (ClassroomsTimetable, ClassroomsTimetable) classroomsTimetable, 
        Classroom classroom, ScheduleTimeRange timeRange)
    {
        var (first, second) = classroomsTimetable;
        return first.RoomIsFree(classroom, timeRange) && second.RoomIsFree(classroom, timeRange);
    }
    
    /// <summary>
    /// This function calculates the total free hours of the rooms
    /// </summary>
    public static int TotalFreeHoursOfRooms(this (ClassroomsTimetable, ClassroomsTimetable) classroomsTimetable)
    {
        var (first, second) = classroomsTimetable;
        var totalHours = ScheduleTime.GetWeekScheduleTimes().Count();

        return first.ClassroomsData.AllClassrooms.Values
            .Where(x => x.RoomType is not (RoomType.WithComputers or RoomType.Laboratory or RoomType.Gym))
            .Sum(x => totalHours - (first.TakenHoursByClassroom(x) + second.TakenHoursByClassroom(x)));
    }
}
