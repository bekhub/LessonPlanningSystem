using FluentAssertions;
using LPS.PlanGenerators.Enums;
using LPS.PlanGenerators.ValueObjects;
using Xunit;

namespace LPS.PlanGenerators.Tests.ValueObjects;

public class ScheduleTimeTests
{
    [Fact]
    public void TestEquality()
    {
        var f = new ScheduleTime(Weekdays.Friday, 1);
        var s = new ScheduleTime(Weekdays.Friday, 1);
        var t = new ScheduleTime(Weekdays.Friday, 2);
        f.Should().BeEquivalentTo(s);
        s.Should().NotBeEquivalentTo(t);
        Assert.True(f == s);
        Assert.True(s != t);
    }
}
