using LessonPlanningSystem.PlanGenerators.Enums;

namespace LessonPlanningSystem.PlanGenerators.Models;

public class Classroom
{
    public Classroom()
    {
        CourseVsRooms = new HashSet<CourseVsRoom>();
        TimeTables = new HashSet<Timetable>();
    }

    public int Id { get; set; }
    public int Capacity { get; set; }
    public int BuildingId { get; set; }
    public int DepartmentId { get; set; }
    public RoomType RoomType { get; set; }

    public Building Building { get; set; }
    public Department Department { get; set; }
    public ICollection<CourseVsRoom> CourseVsRooms { get; set; }
    public ICollection<Timetable> TimeTables { get; set; }

    public int GetRoomTime(int hour)
    {
        throw new NotImplementedException();
    }

    public void SetRoomTime(int hour, int lessonPlanCourseId)
    {
        throw new NotImplementedException();
    }
}
