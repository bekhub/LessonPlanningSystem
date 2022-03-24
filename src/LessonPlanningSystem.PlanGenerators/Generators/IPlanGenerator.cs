using LessonPlanningSystem.PlanGenerators.DataStructures;

namespace LessonPlanningSystem.PlanGenerators.Generators;

public interface IPlanGenerator
{
    TimetableData GenerateLessonPlan(CoursesList coursesList);
}
