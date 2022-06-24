using LPS.PlanGenerators.DataStructures.Timetables;
using LPS.PlanGenerators.Models;

namespace LPS.PlanGenerators.DataStructures;

/// <summary>
/// Timetable that pulled from database
/// </summary>
public class ExistingTimetable
{
    /// <summary>
    /// Timetables by course id
    /// </summary>
    public readonly CoursesTimetable CoursesTimetable;
    /// <summary>
    /// Timetables by classroom id. There may be two classrooms at the same time
    /// </summary>
    public readonly ClassroomsTimetable ClassroomsTimetable;
    /// <summary>
    /// Timetables by teacher code. Teacher can be in only one room at the same time
    /// </summary>
    public readonly TeachersTimetable TeachersTimetable;
    /// <summary>
    /// Timetables by students(department id and grade year). Students can be in multiple rooms at the same time
    /// </summary>
    public readonly StudentsTimetable StudentsTimetable;

    public ExistingTimetable(CoursesData coursesData, ClassroomsData classroomsData, IEnumerable<Timetable> timetables)
    {
        CoursesTimetable = new CoursesTimetable(coursesData);
        ClassroomsTimetable = new ClassroomsTimetable(classroomsData);
        TeachersTimetable = new TeachersTimetable();
        StudentsTimetable = new StudentsTimetable();
        foreach (var timetable in timetables) {
            CoursesTimetable.Add(timetable.Course.Id, timetable);
            TeachersTimetable.Add(timetable.Course.Teacher.Code, timetable);
            StudentsTimetable.Add((timetable.Course.Department.Id, timetable.Course.GradeYear), timetable);
            ClassroomsTimetable.Add(timetable.Classroom.Id, timetable);
        }
    }
}
