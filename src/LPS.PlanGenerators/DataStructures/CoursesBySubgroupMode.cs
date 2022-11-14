using LPS.PlanGenerators.Enums;
using LPS.PlanGenerators.Models;
using LPS.Utils.Extensions;

namespace LPS.PlanGenerators.DataStructures;

public sealed class CoursesBySubgroupMode
{
    private readonly List<Course> _subgroupMode5N6Courses;
    private readonly List<Course> _subgroupMode4Courses;
    private readonly List<Course> _subgroupMode3Courses;
    private readonly List<Course> _specialCourses;
    private readonly List<Course> _normalCourses;
    
    /// <summary>
    /// Courses ids with <see cref="SubgroupMode.Mode5"/> and <see cref="SubgroupMode.Mode6"/>
    /// </summary>
    public IReadOnlyList<Course> SubgroupMode5N6Courses => _subgroupMode5N6Courses;
    /// <summary>
    /// Courses ids with <see cref="SubgroupMode.Mode4"/>
    /// </summary>
    public IReadOnlyList<Course> SubgroupMode4Courses => _subgroupMode4Courses;
    /// <summary>
    /// Courses ids with <see cref="SubgroupMode.Mode3"/>
    /// </summary>
    public IReadOnlyList<Course> SubgroupMode3Courses => _subgroupMode3Courses;
    /// <summary>
    /// Courses ids that have special requirements for rooms
    /// </summary>
    public IReadOnlyList<Course> SpecialCourses => _specialCourses;
    /// <summary>
    /// Ordinary courses ids
    /// </summary>
    public IReadOnlyList<Course> NormalCourses => _normalCourses;

    public IReadOnlyList<Course> AllCourses => _normalCourses
        .Concat(_specialCourses)
        .Concat(_subgroupMode3Courses)
        .Concat(_subgroupMode4Courses)
        .Concat(_subgroupMode5N6Courses)
        .ToList();

    public CoursesBySubgroupMode()
    {
        _subgroupMode5N6Courses = new List<Course>();
        _subgroupMode4Courses = new List<Course>();
        _subgroupMode3Courses = new List<Course>();
        _specialCourses = new List<Course>();
        _normalCourses = new List<Course>();
    }

    public IReadOnlyList<Course> GenerateRandomizedCoursesList()
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
            case SubgroupMode.Mode5: _subgroupMode5N6Courses.Add(course); 
                break;
            case SubgroupMode.Mode4: _subgroupMode4Courses.Add(course); 
                break;
            case SubgroupMode.Mode3: _subgroupMode3Courses.Add(course); 
                break;
            case SubgroupMode.Mode2:
            case SubgroupMode.Mode1:
            case SubgroupMode.Mode0:
            default: {
                if (course.CourseVsRooms != null && course.CourseVsRooms.Any()) _specialCourses.Add(course);
                else _normalCourses.Add(course);
            } break;
        }
    }
}
