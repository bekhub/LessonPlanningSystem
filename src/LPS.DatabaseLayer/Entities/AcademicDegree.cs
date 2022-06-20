namespace LPS.DatabaseLayer.Entities;

public class AcademicDegree
{
    public int Id { get; set; }
    public string Name { get; set; }

    public ICollection<Course> Courses { get; set; }
}
