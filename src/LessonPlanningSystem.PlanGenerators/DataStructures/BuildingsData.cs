using LessonPlanningSystem.PlanGenerators.Models;

namespace LessonPlanningSystem.PlanGenerators.DataStructures;

public class BuildingsData
{
    private readonly Dictionary<int, Building> _allBuildings;

    public IReadOnlyDictionary<int, Building> AllBuildings => _allBuildings;

    public BuildingsData() {
        _allBuildings = new Dictionary<int, Building>();
    }
}
