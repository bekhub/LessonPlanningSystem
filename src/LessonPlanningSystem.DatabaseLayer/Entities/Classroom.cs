namespace LessonPlanningSystem.DatabaseLayer.Entities;

public class Classroom
{
    public Classroom()
    {
        CourseVsRooms = new HashSet<CourseVsRoom>();
        TimeTables = new HashSet<TimeTable>();
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public int Capacity { get; set; }
    public bool Archived { get; set; }
    public int? BuildingId { get; set; }
    public int? DepartmentId { get; set; }
    public int? RoomTypeId { get; set; }
    public int? UserId { get; set; }

    public Building Building { get; set; }
    public Department Department { get; set; }
    public RoomType RoomType { get; set; }
    public FosUser User { get; set; }
    public ICollection<CourseVsRoom> CourseVsRooms { get; set; }
    public ICollection<TimeTable> TimeTables { get; set; }
}
