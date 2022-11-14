#nullable enable
using AutoMapper;
using LPS.PlanGenerators.Configuration;
using Entities = LPS.DatabaseLayer.Entities;
using Models = LPS.PlanGenerators.Models;

namespace LPS.Application.Mapping;

public sealed class ModelsToEntities : Profile
{
    public ModelsToEntities()
    {
        CreateMap<Models.Timetable, Entities.TimeTable>(MemberList.None)
            .ForMember(x => x.LessonTypeId, expression => 
                expression.MapFrom(x => x.LessonType.AsInt()))
            .ForMember(x => x.LessonType, expression => expression.Ignore())
            .ForMember(x => x.ClassroomId, expression => 
                expression.MapFrom(x => x.Classroom.Id))
            .ForMember(x => x.Classroom, expression => expression.Ignore())
            .ForMember(x => x.CourseId, expression => 
                expression.MapFrom(x => x.Course.Id))
            .ForMember(x => x.Course, expression => expression.Ignore())
            .ForMember(x => x.TimeDayId, expression => 
                expression.MapFrom(x => x.ScheduleTime.Weekday.AsInt()))
            .ForMember(x => x.TimeHourId, expression => 
                expression.MapFrom(x => x.ScheduleTime.Hour + 1));
        
        CreateMap<Models.Timetable, Entities.TimeTablePreview>(MemberList.None)
            .ForMember(x => x.LessonTypeId, expression => 
                expression.MapFrom(x => x.LessonType.AsInt()))
            .ForMember(x => x.LessonType, expression => expression.Ignore())
            .ForMember(x => x.ClassroomId, expression => 
                expression.MapFrom(x => x.Classroom.Id))
            .ForMember(x => x.Classroom, expression => expression.Ignore())
            .ForMember(x => x.CourseId, expression => 
                expression.MapFrom(x => x.Course.Id))
            .ForMember(x => x.Course, expression => expression.Ignore())
            .ForMember(x => x.TimeDayId, expression => 
                expression.MapFrom(x => x.ScheduleTime.Weekday.AsInt()))
            .ForMember(x => x.TimeHourId, expression => 
                expression.MapFrom(x => x.ScheduleTime.Hour + 1));
    }
    
    public static (Entities.TimeTablePreview, Entities.TimeTablePreview?) MapTimetablePreview(Models.Timetable timetable, PlanConfiguration configuration)
    {
        var first = new Entities.TimeTablePreview {
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
        return (first, new Entities.TimeTablePreview {
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

    public static (Entities.TimeTable, Entities.TimeTable?) MapTimetable(Models.Timetable timetable, PlanConfiguration configuration)
    {
        var first = new Entities.TimeTable {
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
        return (first, new Entities.TimeTable {
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
}
