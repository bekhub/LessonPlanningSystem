using LessonPlanningSystem.PlanGenerators.Enums;

namespace LessonPlanningSystem.Application.Mapping;

public static class MapHelper
{
    public static TEnum Parse<TEnum>(int value) where TEnum : struct, Enum
    {
        return Enum.Parse<TEnum>(value.ToString());
    }

    public static int AsInt<TEnum>(this TEnum @enum) where TEnum : struct, Enum
    {
        return Convert.ToInt32(@enum);
    }

    public static Semester ParseSemester(string name) => name switch {
        "Güz" => Semester.Autumn,
        "Bahar" => Semester.Spring,
        _ => throw new ArgumentOutOfRangeException(nameof(name), name, null),
    };

    public static string ToDbValue(this Semester semester) => semester switch {
        Semester.Autumn => "Güz",
        Semester.Spring => "Bahar",
        _ => throw new ArgumentOutOfRangeException(nameof(semester), semester, null)
    };
}
