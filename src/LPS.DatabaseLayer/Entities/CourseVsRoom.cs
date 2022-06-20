namespace LPS.DatabaseLayer.Entities;

public class CourseVsRoom
{
    public int Id { get; set; }
    public int LessonType { get; set; }
    public bool Archived { get; set; }
    public int? ClassroomId { get; set; }
    public int? CourseId { get; set; }
    public int? UserId { get; set; }

    public Classroom Classroom { get; set; }
    public Course Course { get; set; }
    public FosUser User { get; set; }
}
