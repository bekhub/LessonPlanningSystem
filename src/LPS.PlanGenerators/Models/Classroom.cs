using LPS.PlanGenerators.Enums;

namespace LPS.PlanGenerators.Models;

public sealed class Classroom : Model
{
    public int Capacity { get; init; }
    public RoomType RoomType { get; init; }

    public Building Building { get; init; }
    public Department Department { get; init; }

    public override string ToString()
    {
        return $"{Id}";
    }
}
