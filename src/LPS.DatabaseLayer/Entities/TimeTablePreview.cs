namespace LPS.DatabaseLayer.Entities;

public class TimeTablePreview 
{
    public int Id { get; set; }
    public string EducationalYear { get; set; }
    public string Semester { get; set; }
    public DateTime CreatedTime { get; set; }
    public int? ClassroomId { get; set; }
    public int CourseId { get; set; }
    public int? LessonTypeId { get; set; }
    public int? TimeDayId { get; set; }
    public int? TimeHourId { get; set; }
    public int? UserId { get; set; }
    public Classroom Classroom { get; set; }
    public Course Course { get; set; }
    public LessonType LessonType { get; set; }
    public TimeDay TimeDay { get; set; }
    public TimeHour TimeHour { get; set; }
    public FosUser User { get; set; }
}
