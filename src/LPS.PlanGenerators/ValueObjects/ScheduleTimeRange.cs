namespace LPS.PlanGenerators.ValueObjects;

public class ScheduleTimeRange : ValueObject
{
    public ScheduleTime Time { get; }
    public int Duration { get; }

    private static readonly Dictionary<(ScheduleTime, int), ScheduleTimeRange> AllScheduleTimeRanges;

    private readonly List<ScheduleTime> _times = new();

    static ScheduleTimeRange()
    {
        var weekScheduleTimes = ScheduleTime.GetWeekScheduleTimes().ToList();
        AllScheduleTimeRanges = new Dictionary<(ScheduleTime, int), ScheduleTimeRange>(8 * weekScheduleTimes.Count);
        for (int duration = 1; duration <= 8; duration++) {
            foreach (var time in weekScheduleTimes) {
                if (!time.NotLunchOrEndOfDay(duration)) continue;
                AllScheduleTimeRanges.Add((time, duration), new ScheduleTimeRange(time, duration));
            }
        }
    }

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
    public static IEnumerable<ScheduleTimeRange> GetWeekScheduleTimeRanges(int duration)
    {
        foreach (var time in ScheduleTime.GetWeekScheduleTimes()) {
            if (!time.NotLunchOrEndOfDay(duration)) continue;
            if (AllScheduleTimeRanges.TryGetValue((time, duration), out var timeRange)) {
                yield return timeRange;
            } else {
                yield return new ScheduleTimeRange(time, duration);
            }
        }
    }

    public static ScheduleTimeRange GetScheduleTimeRange(ScheduleTime time, int duration)
    {
        return AllScheduleTimeRanges.TryGetValue((time, duration), out var timeRange) 
            ? timeRange 
            : new ScheduleTimeRange(time, duration);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Time;
        yield return Duration;
    }
}
