namespace LessonPlanningSystem.PlanGenerators.Models;

public abstract class Entity : IComparable<Entity>
{
    public virtual int Id { get; init; }

    public int CompareTo(Entity other)
    {
        return Id.CompareTo(other.Id);
    }

    public override int GetHashCode()
    {
        return Id;
    }
}
