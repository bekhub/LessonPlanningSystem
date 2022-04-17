﻿using LessonPlanningSystem.PlanGenerators.Models;
using LessonPlanningSystem.PlanGenerators.ValueObjects;

namespace LessonPlanningSystem.Generator.DataStructures.Timetables;

public class TeachersTimetable : ScheduleTimetableDict<int>
{
    public int MaxTeachingHours()
    {
        if (Count == 0) return 0;
        return Values.Select(x => x.Keys)
            .Max(ScheduleTime.CountMaxContinuousDurationPerDay);
    }
    
    /// <summary>
    /// Checks if the teacher is free at the given time.
    /// </summary>
    /// <param name="teacher"></param>
    /// <param name="time"></param>
    /// <returns>True if the teacher is free</returns>
    public bool TeacherIsFree(Teacher teacher, ScheduleTime time)
    {
        if (!ContainsKey(teacher.Code)) return true;
        return !this[teacher.Code].ContainsKey(time);
    }
}
