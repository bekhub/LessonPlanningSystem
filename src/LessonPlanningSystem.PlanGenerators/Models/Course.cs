﻿using LessonPlanningSystem.PlanGenerators.ValueObjects;
using LessonPlanningSystem.PlanGenerators.Enums;
using static LessonPlanningSystem.PlanGenerators.Configuration.StaticConfiguration;

namespace LessonPlanningSystem.PlanGenerators.Models;

public class Course
{
    public int Id { get; init; }
    public int TheoryHours { get; init; }
    public int PracticeHours { get; init; }
    public int MaxStudentsNumber { get; init; }
    public string Code { get; init; }
    public int Credits { get; init; }
    public SubgroupMode SubgroupMode { get; init; }
    public int UnpositionedTheoryHours { get; init; }
    public int UnpositionedPracticeHours { get; init; }
    public Semester Semester { get; init; }
    public GradeYear GradeYear { get; init; }
    public bool? DivideTheoryPractice { get; init; }
    public CourseType CourseType { get; init; }
    public int DepartmentId { get; init; }
    public int FacultyId { get; init; }
    public int TeacherCode { get; init; }
    public RoomType PracticeRoomType { get; init; }
    public RoomType TheoryRoomType { get; init; }

    public Teacher Teacher { get; init; }
    public Faculty Faculty { get; init; }
    public ICollection<CourseVsRoom> CourseVsRooms { get; init; }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="lessonType"></param>
    /// <returns></returns>
    public IEnumerable<int> GetRoomIdsForSpecialCourses(LessonType lessonType) {
        return CourseVsRooms.Where(x => x.LessonType == lessonType)
            .Select(x => x.ClassroomId);
    }

    /// <summary>
    /// Ensures that BZD of 1'st and 3'd level are positioned before noon,
    /// and BZD of 2'nd and 4'th level are positioned after noon
    /// </summary>
    /// <param name="time"></param>
    /// <param name="round"></param>
    /// <returns></returns>
    public bool TimeIsConvenientForCourse(ScheduleTime time, int round) {
        // This rule is not acceptable after first round
        // This rule is not acceptable for the courses other than BZD
        if (round > 2 || CourseType != CourseType.DepartmentMandatory)
            return true;

        // Todo: add check for hoursNeeded
        return GradeYear switch {
            GradeYear.First or GradeYear.Third => time.Hour % HoursPerDay <= 3,
            GradeYear.Second or GradeYear.Fourth => time.Hour % HoursPerDay >= 4,
            _ => true,
        };
    }

    /// <summary>
    /// Room type by lesson type
    /// </summary>
    /// <param name="lessonType"></param>
    /// <returns></returns>
    public RoomType NeededRoomType(LessonType lessonType)
    {
        return lessonType == LessonType.Theory ? TheoryRoomType : PracticeRoomType;
    }
}
