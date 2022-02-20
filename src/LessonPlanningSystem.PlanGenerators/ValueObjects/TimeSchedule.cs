using LessonPlanningSystem.PlanGenerators.Enums;

namespace LessonPlanningSystem.Generator.ValueObjects;

public class TimeSchedule : ValueObject
{
    public Weekdays Weekday { get; }
    public int Hour { get; }
    
    public TimeSchedule(Weekdays weekday, int hour)
    {
        if (hour is < 8 or > 19)
            throw new ArgumentException("Hour must be greater than 8 and less than 19");
        Weekday = weekday;
        Hour = hour;
    }

    public static IEnumerable<TimeSchedule> GetWeekScheduleTimes()
    {
        for (Weekdays wd = 0; wd <= Weekdays.Friday; wd++) {
            for (int h = 1; h <= 12; h++) {
                yield return new TimeSchedule(wd, h);
            }
        }
    }

    private const int HoursPerDay = 12;
    private const int LunchAfterHour = 4;
    
    // This function checks if the given course lessons will not devided by lunch time or end of the day
    //In other words: it checks if the lessons will finish before lunch or before end of the day
    public bool CheckTimeForLunchOrEndOfDay(int hour, int hoursNeeded)
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
