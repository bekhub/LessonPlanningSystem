using LessonPlanningSystem.PlanGenerators.ValueObjects;
using LessonPlanningSystem.PlanGenerators.DataStructures;
using LessonPlanningSystem.PlanGenerators.Configuration;
using LessonPlanningSystem.PlanGenerators.Enums;
using LessonPlanningSystem.PlanGenerators.Models;

namespace LessonPlanningSystem.PlanGenerators.Generators;

public class RandomPlanGenerator : IPlanGenerator
{
    private readonly TimetableData _timetableData;
    private readonly PlanConfiguration _configuration;
    
    public RandomPlanGenerator(PlanConfiguration configuration, ClassroomsData classroomsData)
    {
        _configuration = configuration;
        _timetableData = new TimetableData(classroomsData);
    }

    public TimetableData GenerateLessonPlan(CoursesList coursesList)
    {
        if (_configuration.IncludeRemoteEducationCourses) 
            FindPlaceForRemoteLesson(coursesList.RemoteEducationCourses);
        if (_configuration.IncludeGeneralMandatoryCourses) 
            FindPlaceForGeneralMandatoryLessons(coursesList.GeneralMandatoryCourses);

        // Round 4 - search from same building, round 5 - search from other buildings
        for (var round = Round.First; round <= Round.Fifth; round++) {
            // Placing courses of DSU teachers
            foreach (var course in coursesList.DepartmentMandatoryCoursesLHP) {
                FindPlaceForLesson(course, LessonType.Theory, round); // Find place for TEORIK lesson
                FindPlaceForLesson(course, LessonType.Practice, round); // Find place for UYGULAMA lesson
            }
            foreach (var course in coursesList.DepartmentElectiveCoursesLHP) {
                FindPlaceForLesson(course, LessonType.Theory, round); // Find place for TEORIK lesson
                FindPlaceForLesson(course, LessonType.Practice, round); // Find place for UYGULAMA lesson
            }
            // Placing courses of Tam Zamanli teachers
            foreach (var course in coursesList.DepartmentMandatoryCourses) {
                FindPlaceForLesson(course, LessonType.Theory, round); // Find place for TEORIK lesson
                FindPlaceForLesson(course, LessonType.Practice, round); // Find place for UYGULAMA lesson
            }
            foreach (var course in coursesList.DepartmentElectiveCourses) {
                FindPlaceForLesson(course, LessonType.Theory, round); // Find place for TEORIK lesson
                FindPlaceForLesson(course, LessonType.Practice, round); // Find place for UYGULAMA lesson
            }
        }
        
        return _timetableData;
    }

    private void FindPlaceForLesson(Course course, LessonType lessonType, Round round)
    {
        var currSgMode = course.SubgroupMode;
        if (currSgMode == SubgroupMode.Mode1 && lessonType == LessonType.Practice) {
            var hoursNeeded = _timetableData.RemainingHoursByLessonType(course, lessonType);
            if (hoursNeeded <= 0 || 
                // The following case needed in case if there is only 1 teorik hour and it has been added to uygulama hours in the method RemainingHoursByLessonType
                hoursNeeded % course.PracticeHours != 0) return;
            foreach (var timeRange in ScheduleTimeRange.GetWeekScheduleTimeRanges(hoursNeeded)) {
                if (!_timetableData.ScheduleTimeRangeIsFree(course, timeRange, round) ||
                    // Todo: why we do not use following?!
                    // !course.TimeRangeIsConvenientForCourse(timeRange, round) ||
                    !course.TimeIsConvenientForCourse(timeRange.GetScheduleTimes().First(), round)) continue;

                var rooms = _timetableData.FindFreeRoomsWithMatchedCapacity(course, lessonType, timeRange, round, 
                    (courseStudentsNumber, roomCapacity) => courseStudentsNumber / 2 <= roomCapacity + 10);
                if (rooms == null || rooms.Count == 0) continue;
                foreach (var time in timeRange.GetScheduleTimes()) {
                    _timetableData.AddTimetable(new Timetable {
                        Course = course,
                        Classrooms = rooms,
                        LessonType = lessonType,
                        ScheduleTime = time,
                    });
                }
                if (_timetableData.RemainingHoursByLessonType(course, lessonType) <= 0) break;
            }
        }
        else if (currSgMode <= SubgroupMode.Mode2 || currSgMode == SubgroupMode.Mode6 && lessonType == LessonType.Practice) {
            var hoursNeeded = _timetableData.RemainingHoursByLessonType(course, lessonType);
            if (hoursNeeded <= 0) return;
            foreach (var timeRange in ScheduleTimeRange.GetWeekScheduleTimeRanges(hoursNeeded)) {
                if (!_timetableData.ScheduleTimeRangeIsFree(course, timeRange, round) ||
                    // !course.TimeRangeIsConvenientForCourse(timeRange, round) ||
                    !course.TimeIsConvenientForCourse(timeRange.GetScheduleTimes().First(), round)) continue;
                
                var rooms = _timetableData.FindFreeRoomsWithMatchedCapacity(course, lessonType, timeRange, round);
                if (rooms == null || rooms.Count == 0) continue;
                foreach (var time in timeRange.GetScheduleTimes()) {
                    _timetableData.AddTimetable(new Timetable {
                        Course = course,
                        Classrooms = rooms,
                        LessonType = lessonType,
                        ScheduleTime = time,
                    });
                }
                if (_timetableData.RemainingHoursByLessonType(course, lessonType) <= 0) break;
            }
        }
    }

    private void FindPlaceForRemoteLesson(IReadOnlyList<Course> coursesListRemoteEducationCourses) { }

    private void FindPlaceForGeneralMandatoryLessons(IReadOnlyList<Course> coursesListGeneralMandatoryCourses) { }
}
