using LessonPlanningSystem.PlanGenerators.Enums;

namespace LessonPlanningSystem.PlanGenerators.Models;

public class Course
{
    public int Id { get; init; }
    private readonly int _theoryHours;
    public int TheoryHours {
        get {
            return _theoryHours;
        }
        init {
            if (value == 1 && PracticeHours is not 0 and < 3 && TheoryRoomType == PracticeRoomType) {
                _theoryHours = 0;
                PracticeHours += value;
            } else {
                _theoryHours = value;
            }
        }
    }
    public int PracticeHours { get; init; }
    public int MaxStudentsNumber { get; init; }
    public string Code { get; init; }
    public int Credits { get; init; }
    public SubgroupMode SubgroupMode { get; init; }
    public int UnpositionedTheoryHours { get; init; }
    public int UnpositionedPracticeHours { get; init; }
    public Semester Semester { get; init; }
    public int GradeYear { get; init; }
    public bool? DivideTheoryPractice { get; init; }
    public CourseType CourseType { get; init; }
    public int DepartmentId { get; init; }
    public int FacultyId { get; init; }
    public int TeacherCode { get; init; }
    public RoomType PracticeRoomType { get; init; }
    public RoomType TheoryRoomType { get; init; }

    public Teacher Teacher { get; init; }
    public Faculty Faculty { get; init; }
    public ICollection<CourseVsRoom> CourseVsRooms { get; init; }
    
    public List<int> GetRoomIdsForSpecialCourses(LessonType lessonType) {
        return CourseVsRooms.Where(x => x.LessonType == lessonType).Select(x => x.ClassroomId).ToList();
    }
}
