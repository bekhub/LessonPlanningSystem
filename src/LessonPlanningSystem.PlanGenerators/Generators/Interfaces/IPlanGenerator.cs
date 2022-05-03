using LessonPlanningSystem.PlanGenerators.DataStructures;

namespace LessonPlanningSystem.PlanGenerators.Generators.Interfaces;

public interface IPlanGenerator
{
    TimetableData GenerateLessonPlan(CoursesList coursesList);
}
