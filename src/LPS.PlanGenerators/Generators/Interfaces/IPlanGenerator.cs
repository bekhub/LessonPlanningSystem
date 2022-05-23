using LPS.PlanGenerators.DataStructures;

namespace LPS.PlanGenerators.Generators.Interfaces;

public interface IPlanGenerator
{
    TimetableData GenerateLessonPlan(CoursesList coursesList);
}
