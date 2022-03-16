namespace LessonPlanningSystem.DatabaseLayer.Entities;

public class RoomType
{
    public int Id { get; set; }
    public string Type { get; set; }
    public int TypeCode { get; set; }
    public bool Archived { get; set; }

    public ICollection<Classroom> Classrooms { get; set; }
    public ICollection<Course> CoursePracticeRoomTypes { get; set; }
    public ICollection<Course> CourseTheoryRoomTypes { get; set; }
}
