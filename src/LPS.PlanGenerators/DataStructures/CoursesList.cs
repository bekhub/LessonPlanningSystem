using LPS.PlanGenerators.Models;

namespace LPS.PlanGenerators.DataStructures;

public sealed class CoursesList
{
    public IReadOnlyList<Course> RemoteEducationCourses { get; init; }
    public IReadOnlyList<Course> DepartmentMandatoryCourses { get; init; }
    public IReadOnlyList<Course> DepartmentElectiveCourses { get; init; }
    public IReadOnlyList<Course> GeneralMandatoryCourses { get; init; }
    public IReadOnlyList<Course> DepartmentMandatoryCoursesLHP { get; init; }
    public IReadOnlyList<Course> DepartmentElectiveCoursesLHP { get; init; }

    public IReadOnlyList<Course> AllCourses => RemoteEducationCourses
        .Concat(DepartmentMandatoryCourses)
        .Concat(DepartmentElectiveCourses)
        .Concat(GeneralMandatoryCourses)
        .Concat(DepartmentMandatoryCoursesLHP)
        .Concat(DepartmentElectiveCoursesLHP)
        .ToList();

    public IEnumerable<Course> MainCourses => DepartmentMandatoryCoursesLHP
        .Concat(DepartmentElectiveCoursesLHP)
        .Concat(DepartmentMandatoryCourses)
        .Concat(DepartmentElectiveCourses);
}
