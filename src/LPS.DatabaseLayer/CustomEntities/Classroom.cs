namespace LPS.DatabaseLayer.Entities;

public partial class Classroom
{
    public string DisplayName => $"{Building.ShortName} {Name}";
}
