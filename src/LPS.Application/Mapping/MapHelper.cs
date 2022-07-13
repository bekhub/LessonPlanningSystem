#nullable enable
using LPS.DatabaseLayer.Entities;
using LPS.PlanGenerators.Configuration;
using LPS.PlanGenerators.Enums;
using LPS.PlanGenerators.Models;

namespace LPS.Application.Mapping;

public static class MapHelper
{
    public static (TimeTablePreview, TimeTablePreview?) MapTimetablePreview(Timetable timetable, PlanConfiguration configuration)
    {
        var first = new TimeTablePreview {
            EducationalYear = configuration.EducationalYear,
            Semester = configuration.Semester.ToDbValue(),
            CreatedTime = DateTime.Now,
            ClassroomId = timetable.Classroom.Id,
            CourseId = timetable.Course.Id,
            LessonTypeId = timetable.LessonType.AsInt(),
            TimeDayId = timetable.ScheduleTime.Weekday.AsInt(),
            TimeHourId = timetable.ScheduleTime.Hour + 1
        };
        if (timetable.AdditionalClassroom == null) return (first, null);
        return (first, new TimeTablePreview {
            EducationalYear = configuration.EducationalYear,
            Semester = configuration.Semester.ToDbValue(),
            CreatedTime = DateTime.Now,
            ClassroomId = timetable.AdditionalClassroom.Id,
            CourseId = timetable.Course.Id,
            LessonTypeId = timetable.LessonType.AsInt(),
            TimeDayId = timetable.ScheduleTime.Weekday.AsInt(),
            TimeHourId = timetable.ScheduleTime.Hour + 1
        });
    }

    public static (TimeTable, TimeTable?) MapTimetable(Timetable timetable, PlanConfiguration configuration)
    {
        var first = new TimeTable {
            EducationalYear = configuration.EducationalYear,
            Semester = configuration.Semester.ToDbValue(),
            CreatedTime = DateTime.Now,
            ClassroomId = timetable.Classroom.Id,
            CourseId = timetable.Course.Id,
            LessonTypeId = timetable.LessonType.AsInt(),
            TimeDayId = timetable.ScheduleTime.Weekday.AsInt(),
            TimeHourId = timetable.ScheduleTime.Hour + 1
        };
        if (timetable.AdditionalClassroom == null) return (first, null);
        return (first, new TimeTable {
            EducationalYear = configuration.EducationalYear,
            Semester = configuration.Semester.ToDbValue(),
            CreatedTime = DateTime.Now,
            ClassroomId = timetable.AdditionalClassroom.Id,
            CourseId = timetable.Course.Id,
            LessonTypeId = timetable.LessonType.AsInt(),
            TimeDayId = timetable.ScheduleTime.Weekday.AsInt(),
            TimeHourId = timetable.ScheduleTime.Hour + 1
        });
    }
    
    public static TEnum Parse<TEnum>(int value) where TEnum : struct, Enum
    {
        return Enum.Parse<TEnum>(value.ToString());
    }

    public static int AsInt<TEnum>(this TEnum @enum) where TEnum : struct, Enum
    {
        return Convert.ToInt32(@enum);
    }

    public static Semester ParseSemester(string name) => name switch {
        "Güz" => Semester.Autumn,
        "Bahar" => Semester.Spring,
        _ => throw new ArgumentOutOfRangeException(nameof(name), name, null),
    };

    public static string ToDbValue(this Semester semester) => semester switch {
        Semester.Autumn => "Güz",
        Semester.Spring => "Bahar",
        _ => throw new ArgumentOutOfRangeException(nameof(semester), semester, null)
    };
}
