namespace LessonPlanningSystem.DatabaseLayer.Entities;

public class PermissionToEdit
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string Semester { get; set; }

    public ICollection<FosUser> FosUsers { get; set; }
}
