using AutoMapper;
using LPS.Application.Mapping;
using LPS.DatabaseLayer.Entities;
using LPS.PlanGenerators.Enums;
using LPS.PlanGenerators.Models;
using Xunit;
using LessonType = LPS.PlanGenerators.Enums.LessonType;

namespace LPS.Application.Tests.Mapping;

public class MappingTests
{
    [Fact]
    public void TestModelToEntities()
    {
        var config = new MapperConfiguration(cfg => 
            cfg.AddProfile(typeof(ModelsToEntities)));
        config.AssertConfigurationIsValid();
        var mapper = config.CreateMapper();
        var tt = new Timetable(new(), LessonType.Practice, new(Weekdays.Friday, 1), new ());
        var tte = mapper.Map<TimeTable>(tt);
        Assert.Equal(tte.TimeDay.Id, tt.ScheduleTime.Weekday.AsInt());
        Assert.Equal(tte.TimeDay.OrderPosition, tt.ScheduleTime.Weekday.AsInt());
        Assert.Equal(tte.TimeDay.Label, tt.ScheduleTime.Weekday.ToString());
        Assert.Equal(tte.TimeHour.Id, tt.ScheduleTime.Hour);
        Assert.Equal(tte.TimeHour.OrderPosition, tt.ScheduleTime.Hour);
    }
    
    [Fact]
    public void TestEntitiesToModels()
    {
        var config = new MapperConfiguration(cfg => 
            cfg.AddProfile(typeof(EntitiesToModels)));
        config.AssertConfigurationIsValid();
    }
}
