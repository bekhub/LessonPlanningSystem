using LessonPlanningSystem.PlanGenerators.Models;

namespace LessonPlanningSystem.PlanGenerators.DataStructures;

public class LessonPlansData
{
    private readonly Dictionary<int, LessonPlan> _allLessonPlans;

    public IReadOnlyDictionary<int, LessonPlan> AlLessonPlans => _allLessonPlans;
}
