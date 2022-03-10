using System.Collections.Concurrent;
using LessonPlanningSystem.PlanGenerators.Configuration;
using LessonPlanningSystem.PlanGenerators.DataStructures;
using LessonPlanningSystem.PlanGenerators.Generators;

namespace LessonPlanningSystem.Generator;

public class BestPlanGenerator
{
    private readonly PlanConfiguration _configuration;
    private readonly CoursesData _coursesData;
    private readonly ClassroomsData _classroomsData;
    
    public BestPlanGenerator(PlanConfiguration configuration, CoursesData coursesData, ClassroomsData classroomsData)
    {
        _configuration = configuration;
        _coursesData = coursesData;
        _classroomsData = classroomsData;
    }

    public TimetableData GenerateBestLessonPlan()
    {
        var options = new ParallelOptions {
            MaxDegreeOfParallelism = _configuration.MaxNumberOfThreads ?? Environment.ProcessorCount - 1,
        };
        var inefficiencyDict = new ConcurrentDictionary<int, CoursesList>();
        Parallel.For(0, _configuration.NumberOfVariants, options, _ => {
            var randomizedCoursesLists = _coursesData.GenerateRandomizedCoursesLists();
            var planGenerator = new RandomPlanGenerator(_configuration, _classroomsData);
            var lessonPlan = planGenerator.GenerateLessonPlan(randomizedCoursesLists);
            var inefficiency = CalculateInefficiency(
                lessonPlan.TotalUnpositionedLessons(_coursesData.AllCourses.Values),
                lessonPlan.TotalSeparatedLessons(), lessonPlan.MaxTeachingHours());
            inefficiencyDict.TryAdd(inefficiency, randomizedCoursesLists);
        });
        var minInefficiency = inefficiencyDict.Keys.Min();
        var bestCoursesList = inefficiencyDict[minInefficiency];
        var planGenerator = new RandomPlanGenerator(_configuration, _classroomsData);
        return planGenerator.GenerateLessonPlan(bestCoursesList);
    }

    private int CalculateInefficiency(int totalUnpositionedLessons, int totalSeparatedLessons, int maxTeachingHours)
    {
        return totalUnpositionedLessons * _configuration.UnpositionedLessonsCoefficient +
               totalSeparatedLessons * _configuration.SeparatedLessonsCoefficient +
               maxTeachingHours * _configuration.MaxTeachingHoursCoefficient;
    }
}
