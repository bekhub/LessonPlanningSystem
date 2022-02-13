using LessonPlanningSystem.PlanGenerators.Models;

namespace LessonPlanningSystem.PlanGenerators.DataStructures;

public class ClassroomsData
{
    private readonly Dictionary<int, Classroom> _allClassrooms;

    public IReadOnlyDictionary<int, Classroom> AllClassrooms => _allClassrooms;

    public ClassroomsData()
    {
        _allClassrooms = new Dictionary<int, Classroom>();
    }

    public bool Add(Classroom classroom)
    {
        return _allClassrooms.TryAdd(classroom.Id, classroom);
    }
}
