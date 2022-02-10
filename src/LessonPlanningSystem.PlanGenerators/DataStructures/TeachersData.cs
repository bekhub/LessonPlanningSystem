using LessonPlanningSystem.PlanGenerators.Models;

namespace LessonPlanningSystem.PlanGenerators.DataStructures;

public class TeachersData
{
    private readonly Dictionary<int, Teacher> _allTeachers;

    public IReadOnlyDictionary<int, Teacher> AllTeachers => _allTeachers;
}
