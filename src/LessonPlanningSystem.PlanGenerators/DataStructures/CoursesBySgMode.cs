using LessonPlanningSystem.PlanGenerators.Enums;
using LessonPlanningSystem.PlanGenerators.Models;
using LessonPlanningSystem.Utils;

namespace LessonPlanningSystem.PlanGenerators.DataStructures;

public class CoursesBySgMode
{
    private readonly List<int> _subgroupMode5N6Courses;
    private readonly List<int> _subgroupMode4Courses;
    private readonly List<int> _subgroupMode3Courses;
    private readonly List<int> _specialCourses;
    private readonly List<int> _normalCourses;
    
    /// <summary>
    /// Courses ids with <see cref="SubgroupMode.Mode5"/> and <see cref="SubgroupMode.Mode6"/>
    /// </summary>
    public IReadOnlyList<int> SubgroupMode5N6Courses => _subgroupMode5N6Courses;
    /// <summary>
    /// Courses ids with <see cref="SubgroupMode.Mode4"/>
    /// </summary>
    public IReadOnlyList<int> SubgroupMode4Courses => _subgroupMode4Courses;
    /// <summary>
    /// Courses ids with <see cref="SubgroupMode.Mode3"/>
    /// </summary>
    public IReadOnlyList<int> SubgroupMode3Courses => _subgroupMode3Courses;
    /// <summary>
    /// Courses ids that have special requirements for rooms
    /// </summary>
    public IReadOnlyList<int> SpecialCourses => _specialCourses;
    /// <summary>
    /// Ordinary courses ids
    /// </summary>
    public IReadOnlyList<int> NormalCourses => _normalCourses;

    public CoursesBySgMode()
    {
        _subgroupMode5N6Courses = new List<int>();
        _subgroupMode4Courses = new List<int>();
        _subgroupMode3Courses = new List<int>();
        _specialCourses = new List<int>();
        _normalCourses = new List<int>();
    }

    public List<int> GenerateRandomizedCoursesList()
    {
        var allModesCourses = SubgroupMode5N6Courses.ShuffleOrdinary();
        allModesCourses.AddRange(SubgroupMode4Courses.ShuffleOrdinary());
        allModesCourses.AddRange(SubgroupMode3Courses.ShuffleOrdinary());
        allModesCourses.AddRange(SpecialCourses.ShuffleOrdinary());
        allModesCourses.AddRange(NormalCourses.ShuffleOrdinary());
        return allModesCourses;
    }

    public void AddBySubgroupMode(Course course)
    {
        switch (course.SubgroupMode) {
            case SubgroupMode.Mode6:
            case SubgroupMode.Mode5: _subgroupMode5N6Courses.Add(course.Id); 
                break;
            case SubgroupMode.Mode4: _subgroupMode4Courses.Add(course.Id); 
                break;
            case SubgroupMode.Mode3: _subgroupMode3Courses.Add(course.Id); 
                break;
            case SubgroupMode.Mode2:
            case SubgroupMode.Mode1:
            case SubgroupMode.Mode0:
            default: {
                if (course.CourseVsRooms != null && course.CourseVsRooms.Any()) _specialCourses.Add(course.Id);
                else _normalCourses.Add(course.Id);
            } break;
        }
    }
}
