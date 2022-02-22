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

    // This function checks if the given course lessons will not devided by lunch time or end of the day
    //In other words: it checks if the lessons will finish before lunch or before end of the day
    public static bool NotLunchOrEndOfDay(int hour, int hoursNeeded)
    {
        return hour % HoursPerDay <= LunchAfterHour 
            ? hour % HoursPerDay + hoursNeeded - 1 <= LunchAfterHour 
            : hour % HoursPerDay + hoursNeeded - 1 <= HoursPerDay - 1;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Weekday;
        yield return Hour;
    }
}
