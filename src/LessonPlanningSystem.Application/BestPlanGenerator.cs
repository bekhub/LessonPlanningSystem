using System.Collections.Concurrent;
using LessonPlanningSystem.PlanGenerators.Configuration;
using LessonPlanningSystem.PlanGenerators.DataStructures;
using LessonPlanningSystem.PlanGenerators.Generators;

namespace LessonPlanningSystem.Application;

public class BestPlanGenerator
{
    private readonly PlanConfiguration _configuration;
    private readonly CoursesData _coursesData;
    private readonly ClassroomsData _classroomsData;
    
    private readonly BlockingCollection<(int inefficiency, CoursesList coursesList)> _blockingCollection = new();
    private (int inefficiency, CoursesList coursesList)? _bestCoursesList;
    
    public BestPlanGenerator(PlanConfiguration configuration, CoursesData coursesData, ClassroomsData classroomsData)
    {
        _configuration = configuration;
        _coursesData = coursesData;
        _classroomsData = classroomsData;
    }

    public async Task<(TimetableData timetable, CoursesList coursesList)> GenerateBestLessonPlanAsync()
    {
        var coursesListTask = Task.Run(ChooseBestCoursesList);
        GenerateLessonPlans();
        await coursesListTask;
        if (_bestCoursesList == null) throw new Exception("Best courses list is null");
        var planGenerator = new RandomPlanGenerator(_configuration, _coursesData.AllCourses, _classroomsData);
        var timetable = planGenerator.GenerateLessonPlan(_bestCoursesList.Value.coursesList);
        return (timetable, _bestCoursesList.Value.coursesList);
    }
    
    private void GenerateLessonPlans()
    {
        var options = new ParallelOptions {
            MaxDegreeOfParallelism = _configuration.MaxNumberOfThreads ?? Environment.ProcessorCount - 1,
        };
        try {
            Parallel.For(0, _configuration.NumberOfVariants, options, _ => {
                var randomizedCoursesLists = _coursesData.GenerateRandomizedCoursesLists();
                var planGenerator = new RandomPlanGenerator(_configuration, _coursesData.AllCourses, _classroomsData);
                var lessonPlan = planGenerator.GenerateLessonPlan(randomizedCoursesLists);
                var inefficiency = CalculateInefficiency(
                    lessonPlan.TotalUnpositionedLessons(),
                    lessonPlan.TotalSeparatedLessons(), lessonPlan.MaxTeachingHours());
                _blockingCollection.Add((inefficiency, randomizedCoursesLists));
            });
        } finally {
            _blockingCollection.CompleteAdding();
        }
    }

    private void ChooseBestCoursesList()
    {
        foreach (var coursesList in _blockingCollection.GetConsumingEnumerable()) {
            if (_bestCoursesList == null) {
                _bestCoursesList = coursesList;
                continue;
            }
            if (coursesList.inefficiency < _bestCoursesList.Value.inefficiency) {
                _bestCoursesList = coursesList;
            }
        }
    }

    private int CalculateInefficiency(int totalUnpositionedLessons, int totalSeparatedLessons, int maxTeachingHours)
    {
        return totalUnpositionedLessons * _configuration.UnpositionedLessonsCoefficient +
               totalSeparatedLessons * _configuration.SeparatedLessonsCoefficient +
               maxTeachingHours * _configuration.MaxTeachingHoursCoefficient;
    }
}
