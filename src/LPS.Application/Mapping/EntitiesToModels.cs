using AutoMapper;
using LPS.PlanGenerators.Enums;
using LPS.PlanGenerators.ValueObjects;
using Entities = LPS.DatabaseLayer.Entities;
using Models = LPS.PlanGenerators.Models;

namespace LPS.Application.Mapping;

public class EntitiesToModels : Profile
{
    public EntitiesToModels()
    {
        CreateMap<Entities.Course, Models.Course>(MemberList.Destination)
            .ForMember(x => x.MaxStudentsNumber, expression => 
                expression.MapFrom(x => x.MaxStudents))
            .ForMember(x => x.SubgroupMode, expression => 
                expression.MapFrom(x => MapHelper.Parse<SubgroupMode>(x.SubgroupMode)))
            .ForMember(x => x.Semester, expression => 
                expression.MapFrom(x => MapHelper.ParseSemester(x.Semester)))
            .ForMember(x => x.GradeYear, expression => 
                expression.MapFrom(x => MapHelper.Parse<GradeYear>(x.GradeYear)))
            .ForMember(x => x.CourseType, expression => 
                expression.MapFrom(x => MapHelper.Parse<CourseType>(x.CourseType.TypeCode)))
            .ForMember(x => x.PracticeRoomType, expression => 
                expression.MapFrom(x => MapHelper.Parse<RoomType>(x.PracticeRoomType.TypeCode)))
            .ForMember(x => x.TheoryRoomType, expression => 
                expression.MapFrom(x => MapHelper.Parse<RoomType>(x.TheoryRoomType.TypeCode)));

        CreateMap<Entities.Teacher, Models.Teacher>(MemberList.Destination)
            .ForMember(x => x.TeacherType, expression => 
                expression.MapFrom(x => MapHelper.Parse<TeacherType>(x.EmployeeType)));

        CreateMap<Entities.Classroom, Models.Classroom>(MemberList.Destination)
            .ForMember(x => x.RoomType, expression =>
                expression.MapFrom(x => MapHelper.Parse<RoomType>(x.RoomType.TypeCode)));

        CreateMap<Entities.CourseVsRoom, Models.CourseVsRoom>(MemberList.Destination)
            .ForMember(x => x.LessonType, expression =>
                expression.MapFrom(x => MapHelper.Parse<LessonType>(x.LessonType)));

        CreateMap<Entities.TimeTable, Models.Timetable>(MemberList.Destination)
            .ForMember(x => x.LessonType, expression => 
                expression.MapFrom(x => MapHelper.Parse<LessonType>(x.LessonType.Id)))
            .ForMember(x => x.ScheduleTime, expression => 
                expression.MapFrom(x => 
                    new ScheduleTime(MapHelper.Parse<Weekdays>(x.TimeDay.Id), x.TimeHour.Id)));

        CreateMap<Entities.Department, Models.Department>(MemberList.Destination);
        CreateMap<Entities.Faculty, Models.Faculty>(MemberList.Destination);
        CreateMap<Entities.Building, Models.Building>(MemberList.Destination);
    }
}
