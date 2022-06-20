#nullable enable
namespace LPS.PlanGenerators.Models;

public abstract class Entity : IComparable<Entity>
{
    public virtual int Id { get; init; }

    public int CompareTo(Entity? other)
    {
        return Id.CompareTo(other?.Id);
    }
    
    protected bool Equals(Entity other)
    {
        return Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) {
            return false;
        }

        if (ReferenceEquals(this, obj)) {
            return true;
        }

        return obj.GetType() == this.GetType() && Equals((Entity)obj);
    }
    
    public static bool operator==(Entity? first, Entity? second) => first switch {
        null => second is null, 
        not null when second is null => false, 
        _ => first.Equals(second)
    };

    public static bool operator !=(Entity? first, Entity? second)
    {
        return !(first == second);
    }

    public override int GetHashCode()
    {
        return Id;
    }
}
