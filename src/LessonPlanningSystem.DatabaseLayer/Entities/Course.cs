namespace LessonPlanningSystem.DatabaseLayer.Entities;

public class Course
{
    public int Id { get; set; }
    public int TheoryHours { get; set; }
    public int PracticeHours { get; set; }
    public int MaxStudents { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public int Credits { get; set; }
    public bool Active { get; set; }
    public int SubgroupMode { get; set; }
    public int UnpositionedTheoryHours { get; set; }
    public int UnpositionedPracticeHours { get; set; }
    public string Semester { get; set; }
    public int GradeYear { get; set; }
    public bool? DivideTheoryPractice { get; set; }
    public bool Archived { get; set; }
    public int? AcademicDegreeId { get; set; }
    public int? CourseTypeId { get; set; }
    public int? DepartmentId { get; set; }
    public int? LessonTypeId { get; set; }
    public int? PracticeRoomTypeId { get; set; }
    public int? TeacherId { get; set; }
    public int? TheoryRoomTypeId { get; set; }
    public int? UserId { get; set; }

    public AcademicDegree AcademicDegree { get; set; }
    public CourseType CourseType { get; set; }
    public Department Department { get; set; }
    public LessonType LessonType { get; set; }
    public RoomType PracticeRoomType { get; set; }
    public Teacher Teacher { get; set; }
    public RoomType TheoryRoomType { get; set; }
    public FosUser User { get; set; }
    public ICollection<CourseVsRoom> CourseVsRooms { get; set; }
    public ICollection<TimeTable> TimeTables { get; set; }
}
