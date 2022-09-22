namespace LPS.PlanGenerators.Configuration;

public static class StaticConfiguration
{
    public const int HoursPerDay = 12;
    public const int MinDayHour = 0;
    public const int MaxDayHour = 11;
    public const int RadiusAroundBuilding = 4;
    public static int LunchAfterHour { get; set; } = 4;
    public static int HourStart { get; set; } = MinDayHour;
    public static int HourEnd { get; set; } = MaxDayHour;
}
