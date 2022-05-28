﻿using LPS.PlanGenerators.Enums;
using LPS.PlanGenerators.Models;

namespace LPS.PlanGenerators.DataStructures;

public class CoursesData
{
    private readonly Dictionary<int, Course> _allCourses;
    
    /// <summary>
    /// Courses with <see cref="CourseType.RemoteEducation"/> course type (UED)
    /// </summary>
    public readonly CoursesBySgMode RemoteEducationCourses;
    /// <summary>
    /// Courses with <see cref="CourseType.DepartmentMandatory"/> course type (BZD)
    /// </summary>
    public readonly CoursesBySgMode DepartmentMandatoryCourses;
    /// <summary>
    /// Courses with <see cref="CourseType.DepartmentElective"/> course type (BİSD)
    /// </summary>
    public readonly CoursesBySgMode DepartmentElectiveCourses;
    /// <summary>
    /// Courses with <see cref="CourseType.GeneralMandatory"/> course type (OZD)
    /// </summary>
    public readonly CoursesBySgMode GeneralMandatoryCourses;
    /// <summary>
    /// Courses with <see cref="CourseType.DepartmentMandatory"/> course type (BZD) and
    /// <see cref="TeacherType.LessonHourlyPaid"/> teacher type (DSÜ)
    /// </summary>
    public readonly CoursesBySgMode DepartmentMandatoryCoursesLHP;
    /// <summary>
    /// Courses with <see cref="CourseType.DepartmentElective"/> course type (BİSD) and
    /// <see cref="TeacherType.LessonHourlyPaid"/> teacher type (DSÜ)
    /// </summary>
    public readonly CoursesBySgMode DepartmentElectiveCoursesLHP;

    /// <summary>
    /// Dictionary of all courses. For key is id
    /// </summary>
    public IReadOnlyDictionary<int, Course> AllCourses => _allCourses;

    public CoursesData()
    {
        _allCourses = new Dictionary<int, Course>();
        RemoteEducationCourses = new CoursesBySgMode();
        DepartmentMandatoryCourses = new CoursesBySgMode();
        DepartmentElectiveCourses = new CoursesBySgMode();
        GeneralMandatoryCourses = new CoursesBySgMode();
        DepartmentMandatoryCoursesLHP = new CoursesBySgMode();
        DepartmentElectiveCoursesLHP = new CoursesBySgMode();
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
}