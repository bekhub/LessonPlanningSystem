using LPS.PlanGenerators.DataStructures.Timetables;
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
        return first.UnpositionedTheoryHours(course) + second.UnpositionedTheoryHours(course);
    }

    public static int UnpositionedPracticeHours(this (CoursesTimetable, CoursesTimetable) coursesTimetable, Course course)
    {
        var (first, second) = coursesTimetable;
        return first.UnpositionedPracticeHours(course) + second.UnpositionedPracticeHours(course);
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
        var (first, second) = coursesTimetable;
        return first.TotalUnpositionedLessons() + second.TotalUnpositionedLessons();
    }
    
    /// <summary>
    /// This function calculates the total number of unpositioned courses
    /// </summary>
    public static int TotalUnpositionedCourses(this (CoursesTimetable, CoursesTimetable) coursesTimetable)
    {
        var (first, second) = coursesTimetable;
        return first.TotalUnpositionedCourses() + second.TotalUnpositionedCourses();
    }
}
