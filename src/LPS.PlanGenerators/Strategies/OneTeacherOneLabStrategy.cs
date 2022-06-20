using LPS.PlanGenerators.DataStructures.Extensions;
using LPS.PlanGenerators.DataStructures;
using LPS.PlanGenerators.Enums;
using LPS.PlanGenerators.Models;
using LPS.PlanGenerators.Strategies.Interfaces;
using LPS.PlanGenerators.ValueObjects;

namespace LPS.PlanGenerators.Strategies;

public class OneTeacherOneLabStrategy : ILessonPlacingStrategy
{
    private readonly TimetableData _timetableData;
    
    public OneTeacherOneLabStrategy(TimetableData timetableData) {
        _timetableData = timetableData;
    }

    public void FindPlaceForLesson(Course course, LessonType lessonType, Round round)
    {
        var hoursNeeded = _timetableData.RemainingHoursByLessonType(course, lessonType);
        if (hoursNeeded <= 0 || 
            // The following case needed in case if there is only 1 teorik hour and it has been added to uygulama hours
            // in the method RemainingHoursByLessonType
            hoursNeeded % course.PracticeHours != 0) return;
        foreach (var timeRange in ScheduleTimeRange.GetWeekScheduleTimeRanges(hoursNeeded)) {
            if (!_timetableData.ScheduleTimeRangeIsFree(course, timeRange, round) ||
                // We only check the first hour, because we get hours that are not divided by lunch
                !course.TimeIsConvenientForCourse(timeRange.GetScheduleTimes().First(), round)) continue;

            var freeRooms = _timetableData.GetFreeRoomsByCourse(course, lessonType, timeRange, round);
            var matchedRooms = freeRooms.RoomsWithMatchedCapacity(course, lessonType, 
                (courseStudentsNumber, roomCapacity) => courseStudentsNumber / 2 <= roomCapacity + 10);
            if (matchedRooms == null || matchedRooms.Count == 0) continue;
            _timetableData.AddTimetable(course, lessonType, timeRange, matchedRooms);
            if (_timetableData.RemainingHoursByLessonType(course, lessonType) <= 0) break;
        }
    }
}
