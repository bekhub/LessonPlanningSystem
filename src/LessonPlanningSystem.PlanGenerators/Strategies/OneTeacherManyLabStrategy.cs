using LessonPlanningSystem.PlanGenerators.DataStructures;
using LessonPlanningSystem.PlanGenerators.DataStructures.Extensions;
using LessonPlanningSystem.PlanGenerators.Enums;
using LessonPlanningSystem.PlanGenerators.Models;
using LessonPlanningSystem.PlanGenerators.Strategies.Interfaces;
using LessonPlanningSystem.PlanGenerators.ValueObjects;

namespace LessonPlanningSystem.PlanGenerators.Strategies;

public class OneTeacherManyLabStrategy : ILessonPlacingStrategy
{
    private readonly TimetableData _timetableData;
    
    public OneTeacherManyLabStrategy(TimetableData timetableData) {
        _timetableData = timetableData;
    }

    public void FindPlaceForLesson(Course course, LessonType lessonType, Round round)
    {
        var hoursNeeded = _timetableData.RemainingHoursByLessonType(course, lessonType);
        if (hoursNeeded <= 0) return;
        foreach (var timeRange in ScheduleTimeRange.GetWeekScheduleTimeRanges(hoursNeeded)) {
            if (!_timetableData.ScheduleTimeRangeIsFree(course, timeRange, round) ||
                // We only check the first hour, because we get hours that are not divided by lunch
                !course.TimeIsConvenientForCourse(timeRange.GetScheduleTimes().First(), round)) continue;
            
            var freeRooms = _timetableData.GetFreeRoomsByCourse(course, lessonType, timeRange, round);
            var matchedRooms = freeRooms.RoomsWithMatchedCapacity(course, lessonType);
            if (matchedRooms == null || matchedRooms.Count == 0) continue;
            _timetableData.AddTimetable(course, lessonType, timeRange, matchedRooms);
            if (_timetableData.RemainingHoursByLessonType(course, lessonType) <= 0) break;
        }
    }
}
