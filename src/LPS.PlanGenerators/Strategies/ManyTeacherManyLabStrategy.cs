using LPS.PlanGenerators.DataStructures.Extensions;
using LPS.PlanGenerators.DataStructures;
using LPS.PlanGenerators.Enums;
using LPS.PlanGenerators.Models;
using LPS.PlanGenerators.Strategies.Interfaces;
using LPS.PlanGenerators.ValueObjects;
using LPS.Utils.Extensions;

namespace LPS.PlanGenerators.Strategies;

public sealed class ManyTeacherManyLabStrategy : ILessonPlacingStrategy
{
    private readonly TimetableData _timetableData;
    
    public ManyTeacherManyLabStrategy(TimetableData timetableData) {
        _timetableData = timetableData;
    }

    public void FindPlaceForLesson(Course course, LessonType lessonType, Round round)
    {
        var theory = _timetableData.RemainingHoursByLessonType(course, LessonType.Theory);
        var practice = _timetableData.RemainingHoursByLessonType(course, LessonType.Practice);
        if (theory + practice == 0) return;
        
        var mergedCourses = _timetableData.MergeCoursesByCourse(course);
        if (mergedCourses.Count <= 1) return;
        
        var firstCourse = mergedCourses[0];
        var teachers = new HashSet<int>();
        foreach (var mergedCourse in mergedCourses) {
            // Todo: Why we exit from method here
            if (teachers.Contains(mergedCourse.Teacher.Code) ||
                mergedCourse.TheoryHours != firstCourse.TheoryHours ||
                mergedCourse.PracticeHours != firstCourse.PracticeHours ||
                mergedCourse.TheoryRoomType != firstCourse.TheoryRoomType ||
                mergedCourse.PracticeRoomType != firstCourse.PracticeRoomType) return;
            teachers.Add(mergedCourse.Teacher.Code);
        }
        
        for (var type = LessonType.Theory; type <= LessonType.Practice; type++) {
            var hoursNeeded = _timetableData.RemainingHoursByLessonType(firstCourse, type);
            if (hoursNeeded == 0) continue;
            foreach (var timeRange in ScheduleTimeRange.GetWeekScheduleTimeRanges(hoursNeeded)) {
                if (!course.TimeIsConvenientForCourse(timeRange.GetScheduleTimes().First(), round) ||
                    !_timetableData.ScheduleTimeRangeIsFree(course, timeRange, round)) continue;
                var freeRooms = 
                    _timetableData.GetFreeRoomsByCourses(mergedCourses, type, timeRange, round);
                if (freeRooms.Count < mergedCourses.Count) continue;
                var matchedRooms = 
                    freeRooms.RoomsWithMatchedCapacity(mergedCourses, type);
                if (matchedRooms.IsNullOrEmpty()) continue;
                foreach (var (mergedCourse, classrooms) in matchedRooms!) {
                    _timetableData.AddTimetable(mergedCourse, type, timeRange, classrooms);
                }
                break;
            }
        }
    }
}
