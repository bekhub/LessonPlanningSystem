using AutoMapper;
using Entities = LPS.DatabaseLayer.Entities;
using Models = LPS.PlanGenerators.Models;

namespace LPS.Application.Mapping;

public class ModelsToEntities : Profile
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
    }
}
