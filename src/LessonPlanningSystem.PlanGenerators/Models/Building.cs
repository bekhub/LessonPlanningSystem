namespace LessonPlanningSystem.PlanGenerators.Models;

public class Building
{
    public Building()
    {
        Classrooms = new HashSet<Classroom>();
        Faculties = new HashSet<Faculty>();
    }

    public int Id { get; set; }
    public int DistanceNumber { get; set; }

    public ICollection<Classroom> Classrooms { get; set; }
    public ICollection<Faculty> Faculties { get; set; }
}
