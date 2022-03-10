using LessonPlanningSystem.PlanGenerators.DataStructures;
using LessonPlanningSystem.PlanGenerators.Models;

namespace LessonPlanningSystem.PlanGenerators.Generators;

public interface IPlanGenerator
{
    TimetableData GenerateLessonPlan(CoursesList coursesList);
}
