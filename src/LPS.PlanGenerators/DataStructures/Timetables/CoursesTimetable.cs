using LPS.PlanGenerators.Enums;
using LPS.PlanGenerators.Models;
using LPS.PlanGenerators.ValueObjects;

namespace LPS.PlanGenerators.DataStructures.Timetables;

public sealed class CoursesTimetable : ScheduleTimetableDict<int>
{
    public IReadOnlyList<Course> AllCourses { get; }
    public CoursesTimetable(CoursesData coursesData)
    {
        AllCourses = coursesData.MainCourseList;
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

    public int TakenHours(Course course, LessonType lessonType)
    {
        return TryGetValue(course.Id, out var timetable) ? timetable.Values
            .Count(x => x.LessonType == lessonType) : 0;
    }
    
    /// <summary>
    /// This function calculates the total number of unpositioned lessons
    /// </summary>
    public int TotalUnpositionedLessons()
    {
        return AllCourses.Sum(x => UnpositionedPracticeHours(x) + UnpositionedTheoryHours(x));
    }
    
    /// <summary>
    /// This function calculates the total number of unpositioned courses
    /// </summary>
    public int TotalUnpositionedCourses()
    {
        return AllCourses.Count(x => UnpositionedPracticeHours(x) + UnpositionedTheoryHours(x) > 0);
    }
}
