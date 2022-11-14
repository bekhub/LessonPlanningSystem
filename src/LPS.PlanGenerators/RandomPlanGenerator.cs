using LPS.PlanGenerators.Configuration;
using LPS.PlanGenerators.DataStructures;
using LPS.PlanGenerators.DataStructures.Extensions;
using LPS.PlanGenerators.Enums;
using LPS.PlanGenerators.Models;
using LPS.PlanGenerators.Strategies;
using LPS.PlanGenerators.ValueObjects;

namespace LPS.PlanGenerators;

public sealed class RandomPlanGenerator
{
    private readonly TimetableData _timetableData;
    private readonly PlanConfiguration _configuration;
    private readonly StrategyOrchestrator _strategyOrchestrator;
    
    private RandomPlanGenerator(GeneratorServiceProvider provider)
    {
        _configuration = provider.PlanConfiguration;
        _timetableData = provider.GetNewTimetableData();
        _strategyOrchestrator = new StrategyOrchestrator(_timetableData);
    }
    
    public static GeneratedLessonPlan GenerateLessonPlan(GeneratorServiceProvider provider)
    {
        var coursesList = provider.CoursesData.GenerateRandomizedCoursesLists();
        var generator = new RandomPlanGenerator(provider);
        return generator.GenerateLessonPlan(coursesList);
    }
    
    private GeneratedLessonPlan GenerateLessonPlan(CoursesList coursesList)
    {
        if (_configuration.IncludeRemoteEducationCourses)
            FindPlaceForRemoteLessons(coursesList.RemoteEducationCourses);
        if (_configuration.IncludeGeneralMandatoryCourses)
            FindPlaceForGeneralMandatoryLessons(coursesList.GeneralMandatoryCourses);

        // Round 4 - search from same building, round 5 - search from other buildings
        for (var round = Round.First; round <= Round.Fifth; round++) {
            // Placing DSU teachers courses then Tam Zamanli teachers courses
            foreach (var course in coursesList.MainCourses) FindPlaceForLesson(course, round);
        }

        return new GeneratedLessonPlan {
            NewCoursesTimetable = _timetableData.CoursesTimetable.Current,
            NewClassroomsTimetable = _timetableData.ClassroomsTimetable.Current,
            NewTeachersTimetable = _timetableData.TeachersTimetable.Current,
            NewStudentsTimetable = _timetableData.StudentsTimetable.Current,
            AllTimetables = _timetableData.AllTimetables,
            NewTimetables = _timetableData.GeneratedTimetables,
            GeneratedCoursesList = coursesList,
            UnpositionedCourses = _timetableData.CoursesTimetable.UnpositionedCourses(),
            TotalFreeHoursOfRooms = _timetableData.ClassroomsTimetable.TotalFreeHoursOfRooms(),
            TotalUnpositionedLessons = _timetableData.CoursesTimetable.TotalUnpositionedLessons(),
            TotalUnpositionedCourses = _timetableData.CoursesTimetable.TotalUnpositionedCourses(),
            TotalSeparatedLessons = _timetableData.CoursesTimetable.TotalSeparatedLessons(),
            MaxTeachingHours = _timetableData.TeachersTimetable.MaxTeachingHours()
        };
    }
    
    private void FindPlaceForLesson(Course course, Round round)
    {
        var remainingTheoryHours = _timetableData.RemainingHoursByLessonType(course, LessonType.Theory);
        var remainingPracticeHours = _timetableData.RemainingHoursByLessonType(course, LessonType.Practice);
        if (remainingTheoryHours > 0)
            _strategyOrchestrator.ExecuteStrategy(course, LessonType.Theory, round); // Find place for TEORIK lesson
        if (remainingPracticeHours > 0)
            _strategyOrchestrator.ExecuteStrategy(course, LessonType.Practice, round); // Find place for UYGULAMA lesson
    }

    private void FindPlaceForRemoteLessons(IReadOnlyList<Course> courses)
    {
        if (!_configuration.RemoteEducationLessonTime.HasValue) return;
        if (!_configuration.RemoteEducationClassroomId.HasValue) return;
        var chosenTime = _configuration.RemoteEducationLessonTime.Value;
        var chosenRoomId = _configuration.RemoteEducationClassroomId.Value;
        foreach (var course in courses) {
            var hoursNeeded = course.TheoryHours + course.PracticeHours;
            var timeRange = ScheduleTimeRange.GetScheduleTimeRange(chosenTime, hoursNeeded);
            _timetableData.AddTimetableForRemoteCourse(course, LessonType.Theory, timeRange, chosenRoomId);
        }
    }
    
    private void FindPlaceForGeneralMandatoryLessons(IReadOnlyList<Course> courses) { }
}
