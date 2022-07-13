using LPS.PlanGenerators.DataStructures;
using LPS.PlanGenerators.DataStructures.Timetables;
using LPS.PlanGenerators.Models;

namespace LPS.PlanGenerators;

public class GeneratedLessonPlan
{
    public CoursesTimetable CoursesTimetable { get; init; }
    public ClassroomsTimetable ClassroomsTimetable { get; init; }
    public TeachersTimetable TeachersTimetable { get; init; }
    public StudentsTimetable StudentsTimetable { get; init; }
    public IReadOnlyList<Timetable> AllTimetables { get; init; }
    public IReadOnlyList<Timetable> NewTimetables { get; init; }
    public CoursesList GeneratedCoursesList { get; init; }
    public int TotalFreeHoursOfRooms { get; init; }
    public int TotalUnpositionedLessons { get; init; }
    public int TotalUnpositionedCourses { get; init; }
    public int TotalSeparatedLessons { get; init; }
    public int MaxTeachingHours { get; init; }
}
