using LessonPlanningSystem.PlanGenerators.Enums;

namespace LessonPlanningSystem.PlanGenerators.Models;

public class Classroom
{
    public int Id { get; init; }
    public int Capacity { get; init; }
    public RoomType RoomType { get; init; }

    public Building Building { get; init; }
    public Department Department { get; init; }
}
