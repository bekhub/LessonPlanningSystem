using LPS.PlanGenerators.DataStructures.Extensions;
using LPS.PlanGenerators.DataStructures;
using LPS.PlanGenerators.Enums;
using LPS.PlanGenerators.Models;
using LPS.PlanGenerators.Strategies.Interfaces;
using LPS.PlanGenerators.ValueObjects;

namespace LPS.PlanGenerators.Strategies;

public sealed class OneTeacherManyLabStrategy : ILessonPlacingStrategy
{
    private readonly TimetableData _timetableData;
    
    public OneTeacherManyLabStrategy(TimetableData timetableData) {
        _timetableData = timetableData;
    }

    public void FindPlaceForLesson(Course course, LessonType lessonType, Round round)
    {
        var hoursNeeded = _timetableData.RemainingHoursByLessonType(course, lessonType);
        if (hoursNeeded <= 0) return;
        // Todo: this can work incorrectly
        foreach (var timeRange in ScheduleTimeRange.GetWeekScheduleTimeRanges(hoursNeeded)) {
            if (!_timetableData.ScheduleTimeRangeIsFree(course, timeRange, round) ||
                // We only check the first hour, because we get hours that are not divided by lunch
                !course.TimeIsConvenientForCourse(timeRange.GetScheduleTimes().First(), round)) continue;
            
            var freeRooms = _timetableData.GetFreeRoomsByCourse(course, lessonType, timeRange, round);
            var matchedRooms = freeRooms.RoomsWithMatchedCapacity(course, lessonType);
            if (matchedRooms == null) continue;
            _timetableData.AddTimetable(course, lessonType, timeRange, matchedRooms.Value);
            hoursNeeded = _timetableData.RemainingHoursByLessonType(course, lessonType);
            if (hoursNeeded <= 0) break;
        }
    }
}
