﻿using LessonPlanningSystem.PlanGenerators.ValueObjects;
using LessonPlanningSystem.PlanGenerators.Enums;

namespace LessonPlanningSystem.PlanGenerators.Models;

public class Timetable
{
    public int CourseId { get; set; }
    public int? ClassroomId { get; set; }
    public LessonType LessonType { get; set; }
    public ScheduleTime ScheduleTime { get; set; }

    public Classroom Classroom { get; set; }
    public Course Course { get; set; }
    public TimeDay TimeDay { get; set; }
    public TimeHour TimeHour { get; set; }
}
