using LPS.PlanGenerators.Enums;
using LPS.PlanGenerators.Models;
using LPS.PlanGenerators.ValueObjects;

namespace LPS.PlanGenerators.DataStructures.Timetables;

public class ClassroomsTimetable : ScheduleTimetablesDict<int>
{
    public ClassroomsData ClassroomsData { get; }
    
    public ClassroomsTimetable(ClassroomsData classroomsData) {
        ClassroomsData = classroomsData;
    }

    /// <summary>
    /// Checks if the room is free at that time
    /// </summary>
    public bool RoomIsFree(Classroom classroom, ScheduleTime time)
    {
        if (!ContainsKey(classroom.Id)) return true;
        return !this[classroom.Id].ContainsKey(time);
    }
    
    /// <summary>
    /// Checks if the room is free at that time range
    /// </summary>
    public bool RoomIsFree(Classroom classroom, ScheduleTimeRange timeRange)
    {
        return timeRange.GetScheduleTimes().All(x => RoomIsFree(classroom, x));
    }

    public int TakenHoursByClassroom(Classroom classroom)
    {
        return TryGetValue(classroom.Id, out var timetables) ? timetables.Count : 0;
    }
    
    /// <summary>
    /// This function calculates the total free hours of the rooms
    /// </summary>
    public int TotalFreeHoursOfRooms()
    {
        var totalHours = ScheduleTime.GetWeekScheduleTimes().Count();
        return ClassroomsData.AllClassrooms.Values.Where(x => 
                x.RoomType is not (RoomType.WithComputers or RoomType.Laboratory or RoomType.Gym))
            .Sum(x => ContainsKey(x.Id) ? totalHours - this[x.Id].Count : totalHours);
    }
}
