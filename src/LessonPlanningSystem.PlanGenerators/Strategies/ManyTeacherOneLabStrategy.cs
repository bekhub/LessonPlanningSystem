using LessonPlanningSystem.PlanGenerators.DataStructures;
using LessonPlanningSystem.PlanGenerators.DataStructures.Extensions;
using LessonPlanningSystem.PlanGenerators.Enums;
using LessonPlanningSystem.PlanGenerators.Models;
using LessonPlanningSystem.PlanGenerators.Strategies.Interfaces;
using LessonPlanningSystem.PlanGenerators.ValueObjects;

namespace LessonPlanningSystem.PlanGenerators.Strategies;

public class ManyTeacherOneLabStrategy : ILessonPlacingStrategy
{
    private readonly TimetableData _timetableData;
    
    public ManyTeacherOneLabStrategy(TimetableData timetableData) {
        _timetableData = timetableData;
    }

    public void FindPlaceForLesson(Course course, LessonType lessonType, Round round)
    {
        if (_timetableData.RemainingHoursByLessonType(course, LessonType.Practice) == 0) return;
        
        var mergedCourses = _timetableData.MergeCoursesByCourse(course);
        // Mode 3. Wrong subgroup division mode selected! Courses list size does not equal 2
        if (mergedCourses.Count != 2) return;

        var neededPracticeHoursFirst = mergedCourses[0].PracticeHours;
        var neededTheoryHoursFirst = mergedCourses[0].TheoryHours;
        var neededPracticeHoursSecond = mergedCourses[1].PracticeHours;
        var neededTheoryHoursSecond = mergedCourses[1].TheoryHours;
        
        // Mode 3. Wrong subgroup division mode selected! Hours needed of the selected courses does not match.
        if (neededPracticeHoursFirst != neededPracticeHoursSecond || neededTheoryHoursFirst != neededTheoryHoursSecond)
            return;

        var roomListForTheoryLessonsFirst = _timetableData.GetRoomsByCourse(mergedCourses[0], LessonType.Theory, round);
        var roomListForPracticeLessonsFirst = _timetableData.GetRoomsByCourse(mergedCourses[0], LessonType.Practice, round);
        var roomListForTheoryLessonsSecond = _timetableData.GetRoomsByCourse(mergedCourses[1], LessonType.Theory, round);
        var roomListForPracticeLessonsSecond = _timetableData.GetRoomsByCourse(mergedCourses[1], LessonType.Practice, round);
        
        if (roomListForTheoryLessonsFirst.IsNullOrEmpty() || roomListForTheoryLessonsSecond.IsNullOrEmpty() ||
            roomListForPracticeLessonsFirst.IsNullOrEmpty() || roomListForPracticeLessonsSecond.IsNullOrEmpty()) 
            // Mode 3. Could not find any room for Theory or Practice lessons
            return;
        
        // This mode assumes that there is only one specific lab can be used for the uygulama lessons of this course
        if (roomListForPracticeLessonsFirst.Count != 1 || roomListForPracticeLessonsSecond.Count != 1) 
            // Mode 3. Wrong subgroup division mode selected! More that one labs found
            return;

        var roomForPracticeFirst = roomListForPracticeLessonsFirst[0];
        var roomForPracticeSecond = roomListForPracticeLessonsSecond[0];
        if (roomForPracticeFirst != roomForPracticeSecond) 
            // Mode 3. Wrong subgroup division mode selected! Different labs found
            return;
        
        if (mergedCourses[0].Teacher.Code == mergedCourses[1].Teacher.Code) 
            // Mode 3. Wrong subgroup division mode selected! Teachers match!
            return;

        if (neededTheoryHoursFirst == neededPracticeHoursFirst) {
            neededTheoryHoursFirst = _timetableData.RemainingHoursByLessonType(mergedCourses[0], LessonType.Theory);
            neededPracticeHoursFirst = _timetableData.RemainingHoursByLessonType(mergedCourses[0], LessonType.Practice);
            if (neededTheoryHoursFirst == 0) return;
            foreach (var timeRange in ScheduleTimeRange.GetWeekScheduleTimeRanges(neededTheoryHoursFirst * 2)) {
                if (!course.TimeIsConvenientForCourse(timeRange.GetScheduleTimes().First(), round) ||
                    !_timetableData.TeachersTimetable.TeacherIsFree(mergedCourses[0].Teacher, timeRange) ||
                    !_timetableData.TeachersTimetable.TeacherIsFree(mergedCourses[1].Teacher, timeRange) ||
                    // Todo: why we don't check second course
                    !_timetableData.StudentsTimetable.StudentsAreFree(mergedCourses[0], timeRange, round)) continue;
                var freeRoomList = _timetableData.GetFreeRooms(roomListForTheoryLessonsFirst, timeRange);
                var matchedTheoryList = freeRoomList.RoomsWithMatchedCapacity(mergedCourses[0], LessonType.Theory);
                if (matchedTheoryList.IsNullOrEmpty()) continue;
                freeRoomList = _timetableData.GetFreeRooms(roomListForPracticeLessonsSecond, timeRange);
                var matchedPracticeList = freeRoomList.RoomsWithMatchedCapacity(mergedCourses[0], LessonType.Practice);
                if (matchedPracticeList.IsNullOrEmpty()) continue;
                foreach (var time in timeRange.GetScheduleTimes()) {
                    // var timetableFirst = new Timetable(mergedCourses[0], );
                }
            }
        } else {
            
        }
    }
}
