using LPS.PlanGenerators.DataStructures.Timetables;
using LPS.PlanGenerators.Enums;
using LPS.PlanGenerators.Models;
using LPS.PlanGenerators.ValueObjects;

namespace LPS.PlanGenerators.DataStructures.Extensions;

public static class CoursesTimetableExtensions
{
    /// <summary>
    /// Checks if the course is free at that time (may be the practice lesson at the same time but in the other room)
    /// </summary>
    public static bool CourseIsFree(this (CoursesTimetable, CoursesTimetable) coursesTimetable, int courseId, ScheduleTime time)
    {
        var (first, second) = coursesTimetable;
        return first.CourseIsFree(courseId, time) && second.CourseIsFree(courseId, time);
    }
    
    public static int UnpositionedTheoryHours(this (CoursesTimetable, CoursesTimetable) coursesTimetable, Course course)
    {
        var (first, second) = coursesTimetable;
        if (!first.ContainsKey(course.Id) && !second.ContainsKey(course.Id)) return course.TheoryHours;
        var takenHours = first.TakenHours(course, LessonType.Theory) + second.TakenHours(course, LessonType.Theory);
        return course.TheoryHours - takenHours;
    }

    public static int UnpositionedPracticeHours(this (CoursesTimetable, CoursesTimetable) coursesTimetable, Course course)
    {
        var (first, second) = coursesTimetable;
        if (!first.ContainsKey(course.Id) && !second.ContainsKey(course.Id)) return course.PracticeHours;
        var takenHours = first.TakenHours(course, LessonType.Practice) + second.TakenHours(course, LessonType.Practice);
        return course.PracticeHours - takenHours;
    }
    
    /// <summary>
    /// This function calculates the total number of separated lessons
    /// </summary>
    public static int TotalSeparatedLessons(this (CoursesTimetable, CoursesTimetable) coursesTimetable)
    {
        var (first, second) = coursesTimetable;
        return first.TotalSeparatedLessons() + second.TotalSeparatedLessons();
    }
    
    /// <summary>
    /// This function calculates the total number of unpositioned lessons
    /// </summary>
    public static int TotalUnpositionedLessons(this (CoursesTimetable, CoursesTimetable) coursesTimetable)
    {
        var (first, _) = coursesTimetable;
        return first.AllCourses.Sum(x =>
            coursesTimetable.UnpositionedTheoryHours(x) + coursesTimetable.UnpositionedPracticeHours(x));
    }
    
    /// <summary>
    /// This function calculates the total number of unpositioned courses
    /// </summary>
    public static int TotalUnpositionedCourses(this (CoursesTimetable, CoursesTimetable) coursesTimetable)
    {
        var (first, _) = coursesTimetable;
        return first.AllCourses.Count(x =>
            coursesTimetable.UnpositionedTheoryHours(x) + coursesTimetable.UnpositionedPracticeHours(x) > 0);
    }
}
