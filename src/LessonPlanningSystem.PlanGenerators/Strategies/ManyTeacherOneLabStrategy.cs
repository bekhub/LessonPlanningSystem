using LessonPlanningSystem.PlanGenerators.DataStructures;
using LessonPlanningSystem.PlanGenerators.DataStructures.Extensions;
using LessonPlanningSystem.PlanGenerators.Enums;
using LessonPlanningSystem.PlanGenerators.Models;
using LessonPlanningSystem.PlanGenerators.Strategies.Interfaces;
using LessonPlanningSystem.PlanGenerators.ValueObjects;
using LessonPlanningSystem.Utils;

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

        if (roomListForPracticeLessonsFirst[0] != roomListForPracticeLessonsSecond[0]) 
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
                    var subgroup = time.Hour >= neededPracticeHoursFirst ? 1 : 0;
                    var subgroup2 = time.Hour < neededPracticeHoursFirst ? 1 : 0;
                    var timetableFirst = new Timetable(mergedCourses[subgroup], LessonType.Theory, time, 
                        matchedTheoryList![0]);
                    var timetableSecond = new Timetable(mergedCourses[subgroup2], LessonType.Practice, time,
                        matchedPracticeList![0]);
                    _timetableData.AddTimetable(timetableFirst);
                    _timetableData.AddTimetable(timetableSecond);
                }
                break;
            }
        } else {
            neededTheoryHoursFirst = _timetableData.CoursesTimetable.UnpositionedTheoryHours(mergedCourses[0]);
            neededPracticeHoursFirst = _timetableData.CoursesTimetable.UnpositionedPracticeHours(mergedCourses[0]);

            if (neededTheoryHoursFirst != 0) {
                foreach (var timeRange in ScheduleTimeRange.GetWeekScheduleTimeRanges(neededTheoryHoursFirst)) {
                    if (!course.TimeIsConvenientForCourse(timeRange.GetScheduleTimes().First(), round) ||
                        !_timetableData.TeachersTimetable.TeacherIsFree(mergedCourses[0].Teacher, timeRange) ||
                        !_timetableData.TeachersTimetable.TeacherIsFree(mergedCourses[1].Teacher, timeRange) ||
                        // Todo: why we don't check second course
                        !_timetableData.StudentsTimetable.StudentsAreFree(mergedCourses[0], timeRange, round)) continue;
                    
                    var freeRoomList = _timetableData.GetFreeRooms(roomListForTheoryLessonsFirst, timeRange);
                    if (freeRoomList.Count < 2) continue;
                    
                    var matchedTheoryList1 = freeRoomList.RoomsWithMatchedCapacity(mergedCourses[0], LessonType.Theory);
                    if (matchedTheoryList1.IsNullOrEmpty()) continue;
                    freeRoomList = freeRoomList.Where(x => x.Id != matchedTheoryList1![0].Id).ToList();
                    var matchedTheoryList2 = freeRoomList.RoomsWithMatchedCapacity(mergedCourses[1], LessonType.Theory);
                    if (matchedTheoryList2.IsNullOrEmpty()) continue;
                    
                    foreach (var time in timeRange.GetScheduleTimes()) {
                        var timetableFirst = new Timetable(mergedCourses[0], LessonType.Theory, time, 
                            matchedTheoryList1![0]);
                        var timetableSecond = new Timetable(mergedCourses[1], LessonType.Theory, time,
                            matchedTheoryList2![0]);
                        _timetableData.AddTimetable(timetableFirst);
                        _timetableData.AddTimetable(timetableSecond);
                    }
                    break;
                }
            }

            if (neededPracticeHoursFirst != 0) {
                foreach (var mergedCourse in mergedCourses) {
                    foreach (var timeRange in ScheduleTimeRange.GetWeekScheduleTimeRanges(neededPracticeHoursFirst)) {
                        if (!course.TimeIsConvenientForCourse(timeRange.GetScheduleTimes().First(), round) ||
                            !_timetableData.TeachersTimetable.TeacherIsFree(mergedCourse.Teacher, timeRange) ||
                            // Todo: why we don't check second course
                            !_timetableData.StudentsTimetable.StudentsAreFree(mergedCourse, timeRange, round)) continue;
                        
                        var freeRoomList = _timetableData.GetFreeRooms(roomListForPracticeLessonsSecond, timeRange);
                        if (freeRoomList.Count == 0) continue;

                        var matchedPracticeList =
                            freeRoomList.RoomsWithMatchedCapacity(mergedCourse, LessonType.Practice);
                        if (matchedPracticeList.IsNullOrEmpty()) continue;

                        foreach (var time in timeRange.GetScheduleTimes()) {
                            var timetable = new Timetable(mergedCourse, LessonType.Practice, time, 
                                matchedPracticeList![0]);
                            _timetableData.AddTimetable(timetable);
                        }
                        break;
                    }
                }
            }
        }
    }
}
