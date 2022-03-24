using LessonPlanningSystem.PlanGenerators.Enums;
using static LessonPlanningSystem.PlanGenerators.Configuration.StaticConfiguration;

namespace LessonPlanningSystem.PlanGenerators.ValueObjects;

/// <summary>
/// Value object representing the time on weekdays
/// </summary>
public class ScheduleTime : ValueObject
{
    public Weekdays Weekday { get; }
    public int Hour { get; }
    
    public ScheduleTime(Weekdays weekday, int hour)
    {
        if (hour is < 0 or > 12)
            throw new ArgumentException("Hour must be greater than or equal to 0 and less than or equal to 12");
        Weekday = weekday;
        Hour = hour;
    }

    /// <summary>
    /// Gives all hours in weekdays
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<ScheduleTime> GetWeekScheduleTimes()
    {
        for (Weekdays wd = Weekdays.Monday; wd <= Weekdays.Friday; wd++) {
            for (int h = 0; h <= 12; h++) {
                yield return new ScheduleTime(wd, h);
            }
        }
    }

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
        return Hour % HoursPerDay <= LunchAfterHour 
            ? Hour % HoursPerDay + hoursNeeded - 1 <= LunchAfterHour 
            : Hour % HoursPerDay + hoursNeeded - 1 <= HoursPerDay - 1;
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
