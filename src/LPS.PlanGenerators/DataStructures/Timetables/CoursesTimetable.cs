using LPS.PlanGenerators.Enums;
using LPS.PlanGenerators.Models;
using LPS.PlanGenerators.ValueObjects;

namespace LPS.PlanGenerators.DataStructures.Timetables;

public class CoursesTimetable : ScheduleTimetableDict<int>
{
    private readonly IReadOnlyList<Course> _allCourses;
    public CoursesTimetable(CoursesData coursesData)
    {
        _allCourses = coursesData.AllCourseList;
    }
    
    public override void Add(int key, Timetable timetable)
    {
        if (ContainsKey(key)) {
            if (!this[key].TryAdd(timetable.ScheduleTime, timetable))
                Console.WriteLine($"{key} - courseId is already added");
            return;
        }
        this[key] = new ScheduleTimetable {
            [timetable.ScheduleTime] = timetable,
        };
    }
    
    /// <summary>
    /// Checks if the course is free at that time (may be the practice lesson at the same time but in the other room)
    /// </summary>
    /// <param name="courseId"></param>
    /// <param name="time"></param>
    /// <returns>True if the course is free</returns>
    public bool CourseIsFree(int courseId, ScheduleTime time)
    {
        if (!ContainsKey(courseId)) return true;
        return !this[courseId].ContainsKey(time);
    }
    
    public int UnpositionedTheoryHours(Course course)
    {
        if (!ContainsKey(course.Id)) return course.TheoryHours;
        
        return course.TheoryHours - this[course.Id].Values
            .Count(x => x.LessonType == LessonType.Theory);
    }

    public int UnpositionedPracticeHours(Course course)
    {
        if (!ContainsKey(course.Id)) return course.PracticeHours;
            
        return course.PracticeHours - this[course.Id].Values
            .Count(x => x.LessonType == LessonType.Practice);
    }
    
    /// <summary>
    /// This function calculates the total number of separated lessons
    /// </summary>
    public int TotalSeparatedLessons()
    {
        if (Count == 0) return 0;
        return Values.Select(x => x.Keys)
            .Sum(ScheduleTime.CountSeparatedTimesPerDay);
    }
    
    /// <summary>
    /// This function calculates the total number of unpositioned lessons
    /// </summary>
    public int TotalUnpositionedLessons()
    {
        return _allCourses.Sum(x => UnpositionedPracticeHours(x) + UnpositionedTheoryHours(x));
    }
    
    /// <summary>
    /// This function calculates the total number of unpositioned courses
    /// </summary>
    public int TotalUnpositionedCourses()
    {
        return _allCourses.Count(x => UnpositionedPracticeHours(x) + UnpositionedTheoryHours(x) > 0);
    }
}
