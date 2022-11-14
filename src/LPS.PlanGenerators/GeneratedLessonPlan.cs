using LPS.PlanGenerators.DataStructures;
using LPS.PlanGenerators.DataStructures.Timetables;
using LPS.PlanGenerators.Models;

namespace LPS.PlanGenerators;

public sealed class GeneratedLessonPlan
{
    public CoursesTimetable NewCoursesTimetable { get; init; }
    public ClassroomsTimetable NewClassroomsTimetable { get; init; }
    public TeachersTimetable NewTeachersTimetable { get; init; }
    public StudentsTimetable NewStudentsTimetable { get; init; }
    public IReadOnlyList<Timetable> AllTimetables { get; init; }
    public IReadOnlyList<Timetable> NewTimetables { get; init; }
    public CoursesList GeneratedCoursesList { get; init; }
    public IReadOnlyList<Course> UnpositionedCourses { get; set; }
    public int TotalFreeHoursOfRooms { get; init; }
    public int TotalUnpositionedLessons { get; init; }
    public int TotalUnpositionedCourses { get; init; }
    public int TotalSeparatedLessons { get; init; }
    public int MaxTeachingHours { get; init; }
}
