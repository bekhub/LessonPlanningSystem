using System.Collections.Concurrent;
using LessonPlanningSystem.PlanGenerators.Configuration;
using LessonPlanningSystem.PlanGenerators.DataStructures;
using LessonPlanningSystem.PlanGenerators.Generators;

namespace LessonPlanningSystem.Application;

public class BestPlanGenerator
{
    private readonly PlanConfiguration _configuration;
    private CoursesData _coursesData;
    private ClassroomsData _classroomsData;
    private readonly TimetableService _timetableService;
    
    public BestPlanGenerator(PlanConfiguration configuration, TimetableService timetableService)
    {
        _configuration = configuration;
        _timetableService = timetableService;
    }

    public TimetableData GenerateBestLessonPlan()
    {
        if (_coursesData == null || _classroomsData == null) 
            throw new InvalidOperationException($"Should be called after {nameof(ReadRequiredDataAsync)}");
        var inefficiencyDict = GenerateLessonPlans();
        var minInefficiency = inefficiencyDict.Keys.Min();
        var bestCoursesList = inefficiencyDict[minInefficiency];
        var planGenerator = new RandomPlanGenerator(_configuration, _classroomsData);
        return planGenerator.GenerateLessonPlan(bestCoursesList);
    }
    
    public async Task ReadRequiredDataAsync()
    {
        _coursesData = await _timetableService.GetCoursesDataAsync(_configuration.Semester);
        _classroomsData = await _timetableService.GetClassroomsDataAsync(_coursesData.AllCourses.Values.ToList());
    }

    private IDictionary<int, CoursesList> GenerateLessonPlans()
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
        return inefficiencyDict;
    }

    private int CalculateInefficiency(int totalUnpositionedLessons, int totalSeparatedLessons, int maxTeachingHours)
    {
        return totalUnpositionedLessons * _configuration.UnpositionedLessonsCoefficient +
               totalSeparatedLessons * _configuration.SeparatedLessonsCoefficient +
               maxTeachingHours * _configuration.MaxTeachingHoursCoefficient;
    }
}
