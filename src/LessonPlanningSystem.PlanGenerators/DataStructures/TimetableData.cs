using LessonPlanningSystem.PlanGenerators.Enums;
using LessonPlanningSystem.PlanGenerators.Models;

namespace LessonPlanningSystem.PlanGenerators.DataStructures;

public class TimetableData
{
    public Dictionary<int, List<Timetable>> CoursesTimetable = new();
    public Dictionary<int, List<Timetable>> ClassroomsTimetable = new();
    public Dictionary<int, List<Timetable>> TeachersTimetable = new();
    public Dictionary<(int DepartmentId, GradeYear GradeYear), List<Timetable>> StudentsTimetable = new();

    public int GetRoomIdByCourseAndHour(int courseId, int hour)
    {
        throw new NotImplementedException();
    }
    
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
