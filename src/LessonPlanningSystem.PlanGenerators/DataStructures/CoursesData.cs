using LessonPlanningSystem.PlanGenerators.Enums;
using LessonPlanningSystem.PlanGenerators.Models;

namespace LessonPlanningSystem.PlanGenerators.DataStructures;

public class CoursesData
{
    private readonly Dictionary<int, Course> _allCourses;
    
    public readonly CoursesBySgMode RemoteEducationCourses;
    public readonly CoursesBySgMode DepartmentMandatoryCourses;
    public readonly CoursesBySgMode DepartmentElectiveCourses;
    public readonly CoursesBySgMode GeneralMandatoryCourses;
    public readonly CoursesBySgMode DepartmentMandatoryCoursesLessonTimePaid;
    public readonly CoursesBySgMode DepartmentElectiveCoursesLessonTimePaid;

    public IReadOnlyDictionary<int, Course> AllCourses => _allCourses;

    public CoursesData()
    {
        _allCourses = new Dictionary<int, Course>();
        RemoteEducationCourses = new CoursesBySgMode();
        DepartmentMandatoryCourses = new CoursesBySgMode();
        DepartmentElectiveCourses = new CoursesBySgMode();
        GeneralMandatoryCourses = new CoursesBySgMode();
        DepartmentMandatoryCoursesLessonTimePaid = new CoursesBySgMode();
        DepartmentElectiveCoursesLessonTimePaid = new CoursesBySgMode();
    }

    public bool Add(Course course)
    {
        if (!_allCourses.TryAdd(course.Id, course)) return false;

        switch(course.CourseType) {
            case CourseType.DepartmentMandatory when course.Teacher.TeacherType == TeacherType.LessonTimePaid:
                DepartmentMandatoryCoursesLessonTimePaid.AddBySubgroupMode(course); break;
            case CourseType.DepartmentMandatory: DepartmentMandatoryCourses.AddBySubgroupMode(course); break;
            
            case CourseType.DepartmentElective when course.Teacher.TeacherType == TeacherType.LessonTimePaid:
                DepartmentElectiveCoursesLessonTimePaid.AddBySubgroupMode(course); break;
            case CourseType.DepartmentElective: DepartmentElectiveCourses.AddBySubgroupMode(course); break;

            case CourseType.GeneralMandatory: GeneralMandatoryCourses.AddBySubgroupMode(course); break;
            case CourseType.RemoteEducation: RemoteEducationCourses.AddBySubgroupMode(course); break;
            case CourseType.NonDepartmentalElective:
            default: throw new Exception("Invalid course type!");
        }

        return true;
    }
}
