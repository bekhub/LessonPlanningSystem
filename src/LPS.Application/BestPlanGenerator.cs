using System.Collections.Concurrent;
using LPS.PlanGenerators;
using LPS.PlanGenerators.Configuration;
using LPS.PlanGenerators.Generators;

namespace LPS.Application;

public class BestPlanGenerator
{
    private readonly PlanConfiguration _configuration;
    private readonly ServiceProvider _provider;
    
    private readonly BlockingCollection<(int inefficiency, GeneratedLessonPlan lessonPlan)> _blockingCollection = new();
    private (int inefficiency, GeneratedLessonPlan lessonPlan)? _bestLessonPlan;
    
    public BestPlanGenerator(ServiceProvider serviceProvider)
    {
        _configuration = serviceProvider.PlanConfiguration;
        _provider = serviceProvider;
    }

    public async Task<GeneratedLessonPlan> GenerateBestLessonPlanAsync()
    {
        var coursesListTask = Task.Factory.StartNew(ChooseBestLessonPlan, TaskCreationOptions.LongRunning);
        GenerateLessonPlans();
        await coursesListTask;
        if (_bestLessonPlan == null) throw new Exception("Best courses list is null");
        return _bestLessonPlan.Value.lessonPlan;
    }
    
    private void GenerateLessonPlans()
    {
        var options = new ParallelOptions {
            MaxDegreeOfParallelism = _configuration.MaxNumberOfThreads ?? Environment.ProcessorCount - 1,
        };
        try {
            Parallel.For(0, _configuration.NumberOfVariants, options, _ => {
                var lessonPlan = RandomPlanGenerator.GenerateLessonPlan(_provider);
                var inefficiency = CalculateInefficiency(lessonPlan.TotalUnpositionedLessons, 
                    lessonPlan.TotalSeparatedLessons, lessonPlan.MaxTeachingHours);
                _blockingCollection.Add((inefficiency, lessonPlan));
            });
        } finally {
            _blockingCollection.CompleteAdding();
        }
    }

    private void ChooseBestLessonPlan()
    {
        foreach (var lessonPlan in _blockingCollection.GetConsumingEnumerable()) {
            if (_bestLessonPlan == null) {
                _bestLessonPlan = lessonPlan;
                continue;
            }
            if (lessonPlan.inefficiency < _bestLessonPlan.Value.inefficiency) {
                _bestLessonPlan = lessonPlan;
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
