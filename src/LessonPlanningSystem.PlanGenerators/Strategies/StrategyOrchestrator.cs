using LessonPlanningSystem.PlanGenerators.DataStructures;
using LessonPlanningSystem.PlanGenerators.Enums;
using LessonPlanningSystem.PlanGenerators.Models;
using LessonPlanningSystem.PlanGenerators.Strategies.Interfaces;

namespace LessonPlanningSystem.PlanGenerators.Strategies;

public class StrategyOrchestrator
{
    private readonly OneTeacherOneLabStrategy _oneTeacherOneLabStrategy;
    private readonly OneTeacherManyLabStrategy _oneTeacherManyLabStrategy;
    private readonly ManyTeacherOneLabStrategy _manyTeacherOneLabStrategy;
    private readonly ManyTeacherManyLabStrategy _manyTeacherManyLabStrategy;
    private readonly ManyDepartmentsJointLesson _manyDepartmentsJointLesson;
    
    public StrategyOrchestrator(TimetableData timetableData) {
        _oneTeacherOneLabStrategy = new OneTeacherOneLabStrategy(timetableData);
        _oneTeacherManyLabStrategy = new OneTeacherManyLabStrategy(timetableData);
        _manyTeacherOneLabStrategy = new ManyTeacherOneLabStrategy(timetableData);
        _manyTeacherManyLabStrategy = new ManyTeacherManyLabStrategy(timetableData);
        _manyDepartmentsJointLesson = new ManyDepartmentsJointLesson(timetableData);
    }
    
    public void ExecuteStrategy(Course course, LessonType lessonType, Round round)
    {
        var strategy = GetStrategy(course.SubgroupMode, lessonType);
        strategy.FindPlaceForLesson(course, lessonType, round);
    }
    
    private ILessonPlacingStrategy GetStrategy(SubgroupMode mode, LessonType lessonType)
        => mode switch {
            SubgroupMode.Mode1 when lessonType == LessonType.Practice => _oneTeacherOneLabStrategy,
            SubgroupMode.Mode6 when lessonType == LessonType.Practice => _oneTeacherManyLabStrategy,
            <= SubgroupMode.Mode2 => _oneTeacherManyLabStrategy,
            SubgroupMode.Mode3 => _manyTeacherOneLabStrategy,
            SubgroupMode.Mode4 => _manyTeacherManyLabStrategy,
            SubgroupMode.Mode5 => _manyDepartmentsJointLesson,
            SubgroupMode.Mode6 when lessonType == LessonType.Theory => _manyDepartmentsJointLesson,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, "There is no such strategy!")
        };
}
