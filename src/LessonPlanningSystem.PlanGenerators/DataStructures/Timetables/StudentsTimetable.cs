using LessonPlanningSystem.PlanGenerators.Enums;
using LessonPlanningSystem.PlanGenerators.Models;
using LessonPlanningSystem.PlanGenerators.ValueObjects;

namespace LessonPlanningSystem.Generator.DataStructures.Timetables;

public class StudentsTimetable : ScheduleTimetablesDict<(int DepartmentId, GradeYear gradeYear)>
{
    /// <summary>
    /// Checks if students of the given year level and department are free at the given time
    /// </summary>
    /// <param name="course"></param>
    /// <param name="scheduleTime"></param>
    /// <param name="round"></param>
    /// <returns>True if students are free</returns>
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
}
