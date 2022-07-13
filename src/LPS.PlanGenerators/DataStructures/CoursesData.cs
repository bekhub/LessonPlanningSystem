using LPS.PlanGenerators.Enums;
using LPS.PlanGenerators.Models;

namespace LPS.PlanGenerators.DataStructures;

public class CoursesData
{
    private readonly Dictionary<int, Course> _allCourses;
    
    /// <summary>
    /// Courses with <see cref="CourseType.RemoteEducation"/> course type (UED)
    /// </summary>
    public readonly CoursesBySubgroupMode RemoteEducationCourses;
    /// <summary>
    /// Courses with <see cref="CourseType.DepartmentMandatory"/> course type (BZD)
    /// </summary>
    public readonly CoursesBySubgroupMode DepartmentMandatoryCourses;
    /// <summary>
    /// Courses with <see cref="CourseType.DepartmentElective"/> course type (BİSD)
    /// </summary>
    public readonly CoursesBySubgroupMode DepartmentElectiveCourses;
    /// <summary>
    /// Courses with <see cref="CourseType.GeneralMandatory"/> course type (OZD)
    /// </summary>
    public readonly CoursesBySubgroupMode GeneralMandatoryCourses;
    /// <summary>
    /// Courses with <see cref="CourseType.DepartmentMandatory"/> course type (BZD) and
    /// <see cref="TeacherType.LessonHourlyPaid"/> teacher type (DSÜ)
    /// </summary>
    public readonly CoursesBySubgroupMode DepartmentMandatoryCoursesLHP;
    /// <summary>
    /// Courses with <see cref="CourseType.DepartmentElective"/> course type (BİSD) and
    /// <see cref="TeacherType.LessonHourlyPaid"/> teacher type (DSÜ)
    /// </summary>
    public readonly CoursesBySubgroupMode DepartmentElectiveCoursesLHP;

    /// <summary>
    /// Dictionary of all courses. Key is id
    /// </summary>
    public IReadOnlyDictionary<int, Course> AllCourses => _allCourses;
    public IReadOnlyList<Course> AllCourseList => _allCourses.Values.ToList();

    public CoursesData()
    {
        _allCourses = new Dictionary<int, Course>();
        RemoteEducationCourses = new CoursesBySubgroupMode();
        DepartmentMandatoryCourses = new CoursesBySubgroupMode();
        DepartmentElectiveCourses = new CoursesBySubgroupMode();
        GeneralMandatoryCourses = new CoursesBySubgroupMode();
        DepartmentMandatoryCoursesLHP = new CoursesBySubgroupMode();
        DepartmentElectiveCoursesLHP = new CoursesBySubgroupMode();
    }

    public CoursesList GenerateRandomizedCoursesLists()
    {
        return new CoursesList {
            RemoteEducationCourses = RemoteEducationCourses.GenerateRandomizedCoursesList(),
            DepartmentMandatoryCourses = DepartmentMandatoryCourses.GenerateRandomizedCoursesList(),
            DepartmentElectiveCourses = DepartmentElectiveCourses.GenerateRandomizedCoursesList(),
            GeneralMandatoryCourses = GeneralMandatoryCourses.GenerateRandomizedCoursesList(),
            DepartmentMandatoryCoursesLHP = DepartmentMandatoryCoursesLHP.GenerateRandomizedCoursesList(),
            DepartmentElectiveCoursesLHP = DepartmentElectiveCoursesLHP.GenerateRandomizedCoursesList(),
        };
    }

    public bool Add(Course course)
    {
        if (!_allCourses.TryAdd(course.Id, course)) return false;

        switch(course.CourseType) {
            case CourseType.DepartmentMandatory when course.Teacher.TeacherType == TeacherType.LessonHourlyPaid:
                DepartmentMandatoryCoursesLHP.AddBySubgroupMode(course); break;
            case CourseType.DepartmentMandatory: DepartmentMandatoryCourses.AddBySubgroupMode(course); break;
            
            case CourseType.DepartmentElective when course.Teacher.TeacherType == TeacherType.LessonHourlyPaid:
                DepartmentElectiveCoursesLHP.AddBySubgroupMode(course); break;
            case CourseType.DepartmentElective: DepartmentElectiveCourses.AddBySubgroupMode(course); break;

            case CourseType.GeneralMandatory: GeneralMandatoryCourses.AddBySubgroupMode(course); break;
            case CourseType.RemoteEducation: RemoteEducationCourses.AddBySubgroupMode(course); break;
            case CourseType.NonDepartmentalElective: break; // skip this course type
            default: throw new ArgumentOutOfRangeException(nameof(course.CourseType), "Invalid course type!");
        }

        return true;
    }
    
    /// <summary>
    /// Merges courses by course code, faculty, and subgroup merge mode
    /// </summary>
    public IReadOnlyList<Course> MergeCoursesByCourse(Course course)
    {
        return AllCourseList.Where(x =>
            x.Code == course.Code &&
            x.Department.Faculty.Id == course.Department.Faculty.Id &&
            x.SubgroupMode == course.SubgroupMode).ToList();
    }
}
