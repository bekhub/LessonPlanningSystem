#nullable enable
namespace LPS.PlanGenerators.Models;

public abstract class Model : IComparable<Model>
{
    public virtual int Id { get; init; }

    public int CompareTo(Model? other)
    {
        return Id.CompareTo(other?.Id);
    }
    
    protected bool Equals(Model other)
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

        return obj.GetType() == this.GetType() && Equals((Model)obj);
    }
    
    public static bool operator==(Model? first, Model? second) => first switch {
        null => second is null, 
        not null when second is null => false, 
        _ => first.Equals(second)
    };

    public static bool operator !=(Model? first, Model? second)
    {
        return !(first == second);
    }

    public override int GetHashCode()
    {
        return Id;
    }
}
