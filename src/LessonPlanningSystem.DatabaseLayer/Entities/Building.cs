namespace LessonPlanningSystem.DatabaseLayer.Entities;

public class Building
{
    public Building()
    {
        Classrooms = new HashSet<Classroom>();
        Faculties = new HashSet<Faculty>();
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string ShortName { get; set; }
    public int DistanceNumber { get; set; }
    public bool Archived { get; set; }

    public ICollection<Classroom> Classrooms { get; set; }
    public ICollection<Faculty> Faculties { get; set; }
}
