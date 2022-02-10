using LessonPlanningSystem.PlanGenerators.Models;

namespace LessonPlanningSystem.PlanGenerators.DataStructures;

public class ClassroomsData
{
    private readonly Dictionary<int, Classroom> _allClassrooms;

    public IReadOnlyDictionary<int, Classroom> AllClassrooms => _allClassrooms;
}
