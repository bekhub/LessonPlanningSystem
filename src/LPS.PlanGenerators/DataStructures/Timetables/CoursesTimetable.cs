using LPS.PlanGenerators.Enums;
using LPS.PlanGenerators.Models;
using LPS.PlanGenerators.ValueObjects;

namespace LPS.PlanGenerators.DataStructures.Timetables;

public class CoursesTimetable : ScheduleTimetableDict<int>
{
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
    
    /// <summary>
    /// This function calculates the total number of separated lessons
    /// </summary>
    /// <returns></returns>
    public int TotalSeparatedLessons()
    {
        if (Count == 0) return 0;
        return Values.Select(x => x.Keys)
            .Sum(ScheduleTime.CountSeparatedTimesPerDay);
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
}
