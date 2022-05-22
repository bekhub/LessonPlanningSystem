using LessonPlanningSystem.PlanGenerators.Strategies;
using LessonPlanningSystem.PlanGenerators.DataStructures;
using LessonPlanningSystem.PlanGenerators.Configuration;
using LessonPlanningSystem.PlanGenerators.Enums;
using LessonPlanningSystem.PlanGenerators.Generators.Interfaces;
using LessonPlanningSystem.PlanGenerators.Models;

namespace LessonPlanningSystem.PlanGenerators.Generators;

public class RandomPlanGenerator : IPlanGenerator
{
    private readonly TimetableData _timetableData;
    private readonly PlanConfiguration _configuration;
    private readonly StrategyOrchestrator _strategyOrchestrator;
    
    public RandomPlanGenerator(PlanConfiguration configuration, IReadOnlyDictionary<int, Course> allCourses,
        ClassroomsData classroomsData)
    {
        _configuration = configuration;
        _timetableData = new TimetableData(classroomsData, allCourses.Values.ToList());
        _strategyOrchestrator = new StrategyOrchestrator(_timetableData);
    }

    public TimetableData GenerateLessonPlan(CoursesList coursesList)
    {
        if (_configuration.IncludeRemoteEducationCourses) 
            FindPlaceForRemoteLesson(coursesList.RemoteEducationCourses);
        if (_configuration.IncludeGeneralMandatoryCourses) 
            FindPlaceForGeneralMandatoryLessons(coursesList.GeneralMandatoryCourses);

        // Round 4 - search from same building, round 5 - search from other buildings
        for (var round = Round.First; round <= Round.Fifth; round++) {
            // Placing DSU teachers courses then Tam Zamanli teachers courses
            foreach (var course in coursesList.MainCourses) FindPlaceForLesson(course, round);
        }
        
        return _timetableData;
    }

    private void FindPlaceForLesson(Course course, Round round)
    {
        if (_timetableData.RemainingHoursByLessonType(course, LessonType.Theory) > 0)
            _strategyOrchestrator.ExecuteStrategy(course, LessonType.Theory, round); // Find place for TEORIK lesson
        if (_timetableData.RemainingHoursByLessonType(course, LessonType.Practice) > 0)
            _strategyOrchestrator.ExecuteStrategy(course, LessonType.Practice, round); // Find place for UYGULAMA lesson
    }

    private void FindPlaceForRemoteLesson(IReadOnlyList<Course> coursesListRemoteEducationCourses) { }

    private void FindPlaceForGeneralMandatoryLessons(IReadOnlyList<Course> coursesListGeneralMandatoryCourses) { }
}
