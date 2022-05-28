using LessonPlanningSystem.PlanGenerators.DataStructures;
using LessonPlanningSystem.PlanGenerators.DataStructures.Extensions;
using LessonPlanningSystem.PlanGenerators.Enums;
using LessonPlanningSystem.PlanGenerators.Models;
using LessonPlanningSystem.PlanGenerators.Strategies.Interfaces;
using LessonPlanningSystem.PlanGenerators.ValueObjects;
using LessonPlanningSystem.Utils;

namespace LessonPlanningSystem.PlanGenerators.Strategies;

public class ManyDepartmentsJointLesson : ILessonPlacingStrategy
{
    private readonly TimetableData _timetableData;
    
    public ManyDepartmentsJointLesson(TimetableData timetableData) {
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
        var totalStudentsNumber = 0;
        foreach (var mergedCourse in mergedCourses) {
            // Todo: Why we exit from method here
            if (mergedCourse.Teacher.Code != firstCourse.Teacher.Code ||
                mergedCourse.TheoryHours != firstCourse.TheoryHours ||
                mergedCourse.PracticeHours != firstCourse.PracticeHours ||
                mergedCourse.TheoryRoomType != firstCourse.TheoryRoomType ||
                mergedCourse.PracticeRoomType != firstCourse.PracticeRoomType) return;
            totalStudentsNumber += mergedCourse.MaxStudentsNumber;
        }

        var maxLessonType = course.SubgroupMode == SubgroupMode.Mode6 && lessonType == LessonType.Theory 
            ? LessonType.Theory : LessonType.Practice;
        for (var type = LessonType.Theory; type <= maxLessonType; type++) {
            var hoursNeeded = _timetableData.RemainingHoursByLessonType(firstCourse, type);
            if (hoursNeeded == 0) continue;
            foreach (var timeRange in ScheduleTimeRange.GetWeekScheduleTimeRanges(hoursNeeded)) {
                if (!course.TimeIsConvenientForCourse(timeRange.GetScheduleTimes().First(), round) ||
                    !_timetableData.ScheduleTimeRangeIsFree(firstCourse, timeRange, round)) continue;
                var freeRooms = course.HoursByLessonType(type) != 0 
                    ? _timetableData.GetFreeRoomsByCourses(mergedCourses, type, timeRange, round)
                    : new List<Classroom>();
                if (freeRooms.Count == 0) continue;
                var matchedRooms = freeRooms.RoomsWithMatchedCapacity(firstCourse, type, 
                    (_, roomCapacity) => totalStudentsNumber <= roomCapacity + 10);
                if (matchedRooms.IsNullOrEmpty()) continue;
                _timetableData.AddTimetable(mergedCourses, type, timeRange, matchedRooms!);
                break;
            }
        }
    }
}
