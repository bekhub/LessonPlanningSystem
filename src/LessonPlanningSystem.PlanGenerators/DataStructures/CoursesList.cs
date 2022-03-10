﻿using LessonPlanningSystem.PlanGenerators.Models;

namespace LessonPlanningSystem.PlanGenerators.DataStructures;

public class CoursesList
{
    public IReadOnlyList<Course> RemoteEducationCourses { get; init; }
    public IReadOnlyList<Course> DepartmentMandatoryCourses { get; init; }
    public IReadOnlyList<Course> DepartmentElectiveCourses { get; init; }
    public IReadOnlyList<Course> GeneralMandatoryCourses { get; init; }
    public IReadOnlyList<Course> DepartmentMandatoryCoursesLHP { get; init; }
    public IReadOnlyList<Course> DepartmentElectiveCoursesLHP { get; init; }
}