using LPS.PlanGenerators.Enums;
using LPS.PlanGenerators.ValueObjects;
using Xunit;

namespace LPS.PlanGenerators.Tests.ValueObjects;

public class ScheduleTimeRangeTests
{
    [Fact]
    public void TestEquality()
    {
        var fst = new ScheduleTime(Weekdays.Friday, 1);
        var f = new ScheduleTimeRange(fst, 3);
        var s = new ScheduleTimeRange(fst, 3);
        Assert.True(f == s);
    }
}
