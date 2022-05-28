namespace LPS.PlanGenerators.ValueObjects;

public class ScheduleTimeRange : ValueObject
{
    public ScheduleTime Time { get; }
    public int Duration { get; }

    private readonly List<ScheduleTime> _times = new();

    public ScheduleTimeRange(ScheduleTime time, int duration)
    {
        Time = time;
        Duration = duration;
        for (var i = 0; i < duration; i++) {
            _times.Add(ScheduleTime.GetByWeekAndHour(time.Weekday, time.Hour + i));
        }
    }

    public IEnumerable<ScheduleTime> GetScheduleTimes()
    {
        return _times;
    }

    /// <summary>
    /// Gives all time ranges by duration
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<ScheduleTimeRange> GetWeekScheduleTimeRanges(int duration)
    {
        foreach (var time in ScheduleTime.GetWeekScheduleTimes()) {
            if (!time.NotLunchOrEndOfDay(duration) || time.Hour + duration > 11) continue;
            yield return new ScheduleTimeRange(time, duration);
        }
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Time;
        yield return Duration;
    }
}
