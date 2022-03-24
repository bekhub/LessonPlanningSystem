using System.Threading.Channels;
using LessonPlanningSystem.PlanGenerators.Configuration;
using LessonPlanningSystem.PlanGenerators.DataStructures;
using LessonPlanningSystem.PlanGenerators.Generators;

namespace LessonPlanningSystem.Application;

public class BestPlanGenerator
{
    private readonly PlanConfiguration _configuration;
    private readonly TimetableService _timetableService;
    private readonly Channel<(int inefficiency, CoursesList coursesList)> _channel = Channel.CreateUnbounded<(int, CoursesList)>(new() {
        SingleReader = true,
    });
    private CoursesData _coursesData;
    private ClassroomsData _classroomsData;
    private (int inefficiency, CoursesList coursesList)? _bestCoursesList;
    
    public BestPlanGenerator(PlanConfiguration configuration, TimetableService timetableService)
    {
        _configuration = configuration;
        _timetableService = timetableService;
    }

    public async Task<(TimetableData timetable, CoursesList coursesList)> GenerateBestLessonPlanAsync()
    {
        if (_coursesData == null || _classroomsData == null) 
            throw new InvalidOperationException($"Should be called after {nameof(ReadRequiredDataAsync)}");
        // var generatingTask = Task.Run(GenerateLessonPlansAsync);
        // var coursesListTask = Task.Run(ChooseBestCoursesListAsync);
        await Task.WhenAll(GenerateLessonPlansAsync(), ChooseBestCoursesListAsync());
        if (_bestCoursesList == null) throw new Exception("Best courses list is null");
        var planGenerator = new RandomPlanGenerator(_configuration, _coursesData.AllCourses, _classroomsData);
        var timetable = planGenerator.GenerateLessonPlan(_bestCoursesList.Value.coursesList);
        return (timetable, _bestCoursesList.Value.coursesList);
    }
    
    public async Task ReadRequiredDataAsync()
    {
        _coursesData = await _timetableService.GetCoursesDataAsync(_configuration.Semester);
        _classroomsData = await _timetableService.GetClassroomsDataAsync(_coursesData.AllCourses.Values.ToList());
    }

    private async Task GenerateLessonPlansAsync()
    {
        var options = new ParallelOptions {
            MaxDegreeOfParallelism = _configuration.MaxNumberOfThreads ?? Environment.ProcessorCount - 1,
        };
        try {
            var numberOfVariants = Enumerable.Range(0, _configuration.NumberOfVariants);
            await Parallel.ForEachAsync(numberOfVariants, options, async (_, token) => {
                var randomizedCoursesLists = _coursesData.GenerateRandomizedCoursesLists();
                var planGenerator = new RandomPlanGenerator(_configuration, _coursesData.AllCourses, _classroomsData);
                var lessonPlan = planGenerator.GenerateLessonPlan(randomizedCoursesLists);
                var inefficiency = CalculateInefficiency(
                    lessonPlan.TotalUnpositionedLessons(),
                    lessonPlan.TotalSeparatedLessons(), lessonPlan.MaxTeachingHours());
                await _channel.Writer.WriteAsync((inefficiency, randomizedCoursesLists), token);
            });
        } finally {
            _channel.Writer.Complete();
        }
    }

    private async Task ChooseBestCoursesListAsync()
    {
        await foreach (var coursesList in _channel.Reader.ReadAllAsync()) {
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
