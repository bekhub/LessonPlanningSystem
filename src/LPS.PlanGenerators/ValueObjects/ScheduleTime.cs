using LPS.PlanGenerators.Enums;
using static LPS.PlanGenerators.Configuration.StaticConfiguration;

namespace LPS.PlanGenerators.ValueObjects;

/// <summary>
/// Value object representing the time on weekdays
/// </summary>
public class ScheduleTime : ValueObject
{
    public Weekdays Weekday { get; }
    public int Hour { get; }
    
    private static readonly Dictionary<(Weekdays, int), ScheduleTime> WeekScheduleTimes;
    
    public ScheduleTime(Weekdays weekday, int hour)
    {
        if (hour is < 0 or > 11)
            throw new ArgumentException("Hour must be greater than or equal to 0 and less than or equal to 12");
        Weekday = weekday;
        Hour = hour;
    }

    static ScheduleTime()
    {
        WeekScheduleTimes = new Dictionary<(Weekdays, int), ScheduleTime>(Enum.GetValues<Weekdays>().Length * 12);
        for (Weekdays wd = Weekdays.Monday; wd <= Weekdays.Friday; wd++) {
            for (int h = 0; h <= 11; h++) {
                WeekScheduleTimes.Add((wd, h), new ScheduleTime(wd, h));
            }
        }
    }

    /// <summary>
    /// Gives all hours in weekdays
    /// </summary>
    public static IEnumerable<ScheduleTime> GetWeekScheduleTimes() => WeekScheduleTimes.Values;
    
    /// <summary>
    /// Returns schedule time by weekday and hour. Should be used to not to allocate extra memory
    /// </summary>
    public static ScheduleTime GetByWeekAndHour(Weekdays weekday, int hour) => WeekScheduleTimes[(weekday, hour)];

    public static int CountSeparatedTimesPerDay(IEnumerable<ScheduleTime> times)
    {
        var separatedTimes = 0;
        foreach (var timesByWeekday in times.GroupBy(x => x.Weekday)) {
            var hours = timesByWeekday.Select(x => x.Hour).OrderBy(x => x).ToArray();
            switch (hours.Length) {
                case 1:
                    separatedTimes++; continue;
                case 2:
                    separatedTimes += hours[1] - hours[0] == 1 ? 0 : 1; continue;
            }

            for (var i = 1; i < hours.Length - 1; i++) {
                if (hours[i] - hours[i - 1] == 1 || hours[i + 1] - hours[i] == 1) continue;
                separatedTimes++;
            }
        }

        return separatedTimes;
    }

    public static int CountMaxContinuousDurationPerDay(IEnumerable<ScheduleTime> times)
    {
        var maxContinuousDuration = 0;
        foreach (var timesByWeekday in times.GroupBy(x => x.Weekday)) {
            var hours = timesByWeekday.Select(x => x.Hour).OrderBy(x => x).ToArray();
            switch (hours.Length) {
                case 1:
                    maxContinuousDuration = Math.Max(maxContinuousDuration, 1); 
                    continue;
                case 2:
                    if (hours[1] - hours[0] == 1) maxContinuousDuration = Math.Max(maxContinuousDuration, 2);
                    continue;
            }

            var currentContinuousDuration = 0;
            for (var i = 0; i < hours.Length - 1; i++) {
                if (hours[i + 1] - hours[i] == 1) currentContinuousDuration++;
                else {
                    maxContinuousDuration = Math.Max(maxContinuousDuration, currentContinuousDuration + 1);
                    currentContinuousDuration = 0;
                }
            }
            maxContinuousDuration = Math.Max(maxContinuousDuration, currentContinuousDuration);
        }

        return maxContinuousDuration;
    }
    
    /// <summary>
    /// This function checks if the given course lessons will not devided by lunch time or end of the day
    /// In other words: it checks if the lessons will finish before lunch or before end of the day
    /// </summary>
    /// <param name="hoursNeeded"></param>
    /// <returns></returns>
    public bool NotLunchOrEndOfDay(int hoursNeeded)
    {
        return Hour <= LunchAfterHour 
            ? Hour + hoursNeeded < LunchAfterHour 
            : Hour + hoursNeeded <= HoursPerDay;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Weekday;
        yield return Hour;
    }

    public override string ToString()
    {
        return $"({Weekday} - {Hour})";
    }
}
