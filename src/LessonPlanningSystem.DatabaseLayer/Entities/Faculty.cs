namespace LessonPlanningSystem.DatabaseLayer.Entities;

public class Faculty
{
    public Faculty()
    {
        Departments = new HashSet<Department>();
        FosUsers = new HashSet<FosUser>();
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public bool Archived { get; set; }
    public int? BuildingId { get; set; }

    public Building Building { get; set; }
    public ICollection<Department> Departments { get; set; }
    public ICollection<FosUser> FosUsers { get; set; }
}