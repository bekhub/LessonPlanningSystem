using LessonPlanningSystem.PlanGenerators.Enums;

namespace LessonPlanningSystem.PlanGenerators.Models;

public class LessonPlan
{
    public int CourseId { get; set; }
    public string EducationalYear { get; set; }
    public Semester Semester { get; set; }
    public int ClassroomId { get; set; }
    public LessonType LessonType { get; set; }
    public int UnpositionedTheoryHours { get; set; }
    public int UnpositionedPracticeHours { get; set; }

    public Classroom Classroom { get; set; }
    public Course Course { get; set; }

    public int RemainingHoursByLessonType(LessonType lessonType) => lessonType switch {
        // If there is only 1 hour for teorik lesson, may be it is better to put it together with uygulama lessons
        // Todo: remove side effects in this method
        LessonType.Theory when
            UnpositionedTheoryHours == 1 &&
            Course.PracticeHours is not 0 and < 3 &&
            Course.TheoryRoomType == Course.PracticeRoomType => AddTheoryHoursToPractice(),
        LessonType.Theory => UnpositionedTheoryHours,
        LessonType.Practice => UnpositionedPracticeHours,
        _ => throw new ArgumentOutOfRangeException(nameof(lessonType), lessonType, "Not handled all lesson types!"),
    };

    private int AddTheoryHoursToPractice()
    {
        UnpositionedPracticeHours += UnpositionedTheoryHours;
        UnpositionedTheoryHours = 0;
        return UnpositionedTheoryHours;
    }

    public int GetRoomId(int hour)
    {
        throw new NotImplementedException();
    }

    public void SetCourseTime(int hour, int getRoomId)
    {
        throw new NotImplementedException();
    }
}
