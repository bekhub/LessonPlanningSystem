using LPS.PlanGenerators.Enums;
using LPS.PlanGenerators.Models;
using LPS.PlanGenerators.ValueObjects;

namespace LPS.PlanGenerators.DataStructures.Timetables;

public class StudentsTimetable : ScheduleTimetablesDict<(int DepartmentId, GradeYear gradeYear)>
{
    /// <summary>
    /// Checks if students of the given year level and department are free at the given time
    /// </summary>
    public bool StudentsAreFree(Course course, ScheduleTime scheduleTime, Round round)
    {
        var departmentId = course.Department.Id;
        var gradeYear = course.GradeYear;
        if (!ContainsKey((departmentId, gradeYear))) return true;
        if (!this[(departmentId, gradeYear)].ContainsKey(scheduleTime)) return true;
        var timetablesByDate = this[(departmentId, gradeYear)][scheduleTime];

        if (course.CourseType == CourseType.DepartmentElective && round > Round.First)
            return timetablesByDate.TrueForAll(x => x.Course.CourseType == CourseType.DepartmentElective);
        
        return timetablesByDate.Count == 0;
    }
    
    /// <summary>
    /// Checks if students of the given year level and department are free at the given time
    /// </summary>
    public bool StudentsAreFree(Course course, ScheduleTimeRange timeRange, Round round)
    {
        return timeRange.GetScheduleTimes().All(time => StudentsAreFree(course, time, round));
    }
}
