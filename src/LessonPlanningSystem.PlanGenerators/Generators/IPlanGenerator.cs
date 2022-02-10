using LessonPlanningSystem.PlanGenerators.Models;

namespace LessonPlanningSystem.PlanGenerators.Generators;

public interface IPlanGenerator
{
    LessonPlan GenerateBestLessonPlan();
}
