using LPS.PlanGenerators.Enums;
using LPS.PlanGenerators.ValueObjects;
using static LPS.PlanGenerators.Configuration.StaticConfiguration;

namespace LPS.PlanGenerators.Models;

public class Course : Model
{
    public int TheoryHours { get; init; }
    public int PracticeHours { get; init; }
    public int MaxStudentsNumber { get; init; }
    public string Code { get; init; }
    public int Credits { get; init; }
    public SubgroupMode SubgroupMode { get; init; }
    public Semester Semester { get; init; }
    public GradeYear GradeYear { get; init; }
    public bool? DivideTheoryPractice { get; init; }
    public CourseType CourseType { get; init; }
    public RoomType? PracticeRoomType { get; init; }
    public RoomType? TheoryRoomType { get; init; }

    public Teacher Teacher { get; init; }
    public Department Department { get; init; }
    public IReadOnlyCollection<CourseVsRoom> CourseVsRooms { get; init; }

    private IReadOnlyList<Classroom> _theorySpecialRooms;
    private IReadOnlyList<Classroom> _practiceSpecialRooms;

    /// <summary>
    /// Returns special rooms for this course by lesson type
    /// </summary>
    /// <param name="lessonType">Type of lesson</param>
    /// <returns>Special rooms</returns>
    public IReadOnlyList<Classroom> GetRoomsForSpecialCourses(LessonType lessonType) => lessonType switch {
        LessonType.Theory => _theorySpecialRooms,
        LessonType.Practice => _practiceSpecialRooms,
        _ => throw new ArgumentOutOfRangeException(nameof(lessonType), lessonType, null),
    };

    /// <summary>
    /// Call after object creation
    /// </summary>
    public void CourseCreated()
    {
        GenerateSpecialRooms();
    }

    private void GenerateSpecialRooms()
    {
        _theorySpecialRooms = CourseVsRooms.Where(x => x.LessonType == LessonType.Theory)
            .Where(x => x.Classroom != null)
            .Select(x => x.Classroom).ToList();
        _practiceSpecialRooms = CourseVsRooms.Where(x => x.LessonType == LessonType.Practice)
            .Where(x => x.Classroom != null)
            .Select(x => x.Classroom).ToList();
    }

    /// <summary>
    /// Ensures that BZD of 1'st and 3'd level are positioned before noon,
    /// and BZD of 2'nd and 4'th level are positioned after noon
    /// </summary>
    /// <param name="time"></param>
    /// <param name="round"></param>
    /// <returns></returns>
    public bool TimeIsConvenientForCourse(ScheduleTime time, Round round)
    {
        // This rule is not acceptable after first round
        // This rule is not acceptable for the courses other than BZD
        if (round > Round.Second || CourseType != CourseType.DepartmentMandatory)
            return true;

        return GradeYear switch {
            GradeYear.First or GradeYear.Third => time.Hour % HoursPerDay <= 3,
            GradeYear.Second or GradeYear.Fourth => time.Hour % HoursPerDay >= 4,
            _ => true,
        };
    }

    /// <summary>
    /// Ensures that BZD of 1'st and 3'd level are positioned before noon,
    /// and BZD of 2'nd and 4'th level are positioned after noon
    /// </summary>
    /// <param name="timeRange"></param>
    /// <param name="round"></param>
    /// <returns></returns>
    public bool TimeRangeIsConvenientForCourse(ScheduleTimeRange timeRange, Round round) =>
        timeRange.GetScheduleTimes().All(x => TimeIsConvenientForCourse(x, round));

    /// <summary>
    /// Room type by lesson type
    /// </summary>
    /// <param name="lessonType"></param>
    /// <returns></returns>
    public RoomType? NeededRoomType(LessonType lessonType) => lessonType == LessonType.Theory ? TheoryRoomType : PracticeRoomType;

    /// <summary>
    /// Lesson hours by lesson type
    /// </summary>
    /// <param name="lessonType"></param>
    /// <returns></returns>
    public int HoursByLessonType(LessonType lessonType) => lessonType == LessonType.Theory ? TheoryHours : PracticeHours;
}
