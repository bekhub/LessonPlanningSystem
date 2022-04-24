using LessonPlanningSystem.PlanGenerators.ValueObjects;
using LessonPlanningSystem.PlanGenerators.DataStructures;
using LessonPlanningSystem.PlanGenerators.Configuration;
using LessonPlanningSystem.PlanGenerators.DataStructures.Extensions;
using LessonPlanningSystem.PlanGenerators.Enums;
using LessonPlanningSystem.PlanGenerators.Models;

namespace LessonPlanningSystem.PlanGenerators.Generators;

public class RandomPlanGenerator : IPlanGenerator
{
    private readonly TimetableData _timetableData;
    private readonly ClassroomsData _classroomsData;
    private readonly PlanConfiguration _configuration;
    
    public RandomPlanGenerator(PlanConfiguration configuration, IReadOnlyDictionary<int, Course> allCourses,
        ClassroomsData classroomsData)
    {
        _configuration = configuration;
        _classroomsData = classroomsData;
        _timetableData = new TimetableData(classroomsData, allCourses.Values.ToList());
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
        switch (course.SubgroupMode) {
            case SubgroupMode.Mode1 when lessonType == LessonType.Practice:
                FindPlaceForOneTeacherOneLabLesson(course, lessonType, round); break;
            case SubgroupMode.Mode6 when lessonType == LessonType.Practice:
            case <= SubgroupMode.Mode2: FindPlaceForOneTeacherManyLabLesson(course, lessonType, round); break;
            case SubgroupMode.Mode3: FindPlaceForManyTeacherOneLabLesson(course, lessonType, round); break;
            case SubgroupMode.Mode4: FindPlaceForManyTeacherManyLabLesson(course, lessonType, round); break;
            case SubgroupMode.Mode6 when lessonType == LessonType.Theory: 
            case SubgroupMode.Mode5: FindPlaceForManyDepartmentsJointLesson(course, lessonType, round); break;
        }
    }

    private void FindPlaceForOneTeacherOneLabLesson(Course course, LessonType lessonType, Round round)
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

    private void FindPlaceForOneTeacherManyLabLesson(Course course, LessonType lessonType, Round round)
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

    private void FindPlaceForManyTeacherOneLabLesson(Course course, LessonType lessonType, Round round)
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
            }
        } else {
            
        }
    }
    
    private void FindPlaceForManyTeacherManyLabLesson(Course course, LessonType lessonType, Round round)
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

    private void FindPlaceForManyDepartmentsJointLesson(Course course, LessonType lessonType, Round round)
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

    private void FindPlaceForRemoteLesson(IReadOnlyList<Course> coursesListRemoteEducationCourses) { }

    private void FindPlaceForGeneralMandatoryLessons(IReadOnlyList<Course> coursesListGeneralMandatoryCourses) { }
}
