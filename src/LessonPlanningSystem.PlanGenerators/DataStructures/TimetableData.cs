using LessonPlanningSystem.PlanGenerators.Enums;
using LessonPlanningSystem.PlanGenerators.Models;
using LessonPlanningSystem.PlanGenerators.ValueObjects;

namespace LessonPlanningSystem.PlanGenerators.DataStructures;

public class TimetableData
{
    /// <summary>
    /// Timetables by course id
    /// </summary>
    public Dictionary<int, List<Timetable>> CoursesTimetable = new();
    /// <summary>
    /// Timetables by classroom id
    /// </summary>
    public Dictionary<int, List<Timetable>> ClassroomsTimetable = new();
    /// <summary>
    /// Timetables by teacher code
    /// </summary>
    public Dictionary<int, List<Timetable>> TeachersTimetable = new();
    /// <summary>
    /// Timetables by students(department id and grade year)
    /// </summary>
    public Dictionary<(int DepartmentId, GradeYear GradeYear), List<Timetable>> StudentsTimetable = new();

    /// <summary>
    /// Checks if course, students and teacher are free at the given time
    /// </summary>
    /// <param name="course"></param>
    /// <param name="time"></param>
    /// <param name="hoursNeeded"></param>
    /// <param name="round"></param>
    /// <returns></returns>
    public bool ScheduleTimeIsFree(Course course, ScheduleTime time, int hoursNeeded, int round)
    {
        for (int l = 0; l < hoursNeeded; l++) {
            var currentTime = new ScheduleTime(time.Weekday, time.Hour + l);
            //check if the course free at that time (may be the uygulama lesson at the same time but in the other room)
            if (!CourseIsFree(course.Id, currentTime)) return false;

            // check if the students are free at the given time
            // Todo: keep <round> logic in one place
            if (!StudentsAreFree(course, currentTime, round)) return false;

            // check if the teacher is free at the given time
            if (!TeacherIsFree(course.Teacher, currentTime)) return false;
        }

        return true;
    }
    
    /// <summary>
    /// Checks if the course is free at that time (may be the practice lesson at the same time but in the other room)
    /// </summary>
    /// <param name="courseId"></param>
    /// <param name="time"></param>
    /// <returns>True if the course is free</returns>
    public bool CourseIsFree(int courseId, ScheduleTime time)
    {
        return CoursesTimetable[courseId].TrueForAll(x => x.ScheduleTime != time);
    }

    /// <summary>
    /// Checks if students of the given year level and department are free at the given time
    /// </summary>
    /// <param name="course"></param>
    /// <param name="scheduleTime"></param>
    /// <param name="round"></param>
    /// <returns>True if students are free</returns>
    public bool StudentsAreFree(Course course, ScheduleTime scheduleTime, int round)
    {
        var timetablesByDate = StudentsTimetable[(course.DepartmentId, course.GradeYear)]
            .Where(x => x.ScheduleTime == scheduleTime).ToList();
        
        if (course.CourseType == CourseType.DepartmentElective && round > 1)
            return timetablesByDate.TrueForAll(x => x.Course.CourseType == CourseType.DepartmentElective);
        
        return timetablesByDate.Count == 0;
    }
    
    /// <summary>
    /// Checks if the teacher is free at the given time.
    /// </summary>
    /// <param name="teacher"></param>
    /// <param name="time"></param>
    /// <returns>True if the teacher is free</returns>
    public bool TeacherIsFree(Teacher teacher, ScheduleTime time) {
        return TeachersTimetable[teacher.Code].TrueForAll(x => x.ScheduleTime != time);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="course"></param>
    /// <param name="lessonType"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public int RemainingHoursByLessonType(Course course, LessonType lessonType) => lessonType switch {
        // If there is only 1 hour for teorik lesson, may be it is better to put it together with uygulama lessons
        LessonType.Theory when UnpositionedTheoryHours(course) == 1 && course.PracticeHours is not 0 and < 3 &&
                               course.TheoryRoomType == course.PracticeRoomType => 0,
        LessonType.Practice when UnpositionedTheoryHours(course) == 1 && course.PracticeHours is not 0 and < 3 && 
                                 course.TheoryRoomType == course.PracticeRoomType => UnpositionedPracticeHours(course) + 1,
        
        LessonType.Theory => UnpositionedTheoryHours(course),
        LessonType.Practice => UnpositionedPracticeHours(course),
        _ => throw new ArgumentOutOfRangeException(nameof(lessonType), lessonType, "Not handled all lesson types!"),
    };

    private int UnpositionedTheoryHours(Course course) => 
        course.TheoryHours - CoursesTimetable[course.Id].Count(x => x.LessonType == LessonType.Theory);
    private int UnpositionedPracticeHours(Course course) => 
        course.PracticeHours - CoursesTimetable[course.Id].Count(x => x.LessonType == LessonType.Practice);
}
