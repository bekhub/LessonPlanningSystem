namespace LessonPlanningSystem.Utils;

/// <summary>
/// This is the thread-safe Random number generator. It creates local Random for each thread once.
/// Taken from <a href="https://stackoverflow.com/a/1262619/16976426">here</a>
/// </summary>
public static class ThreadSafeRandom
{
    [ThreadStatic]
    private static Random? _local;

    public static Random ThisThreadsRandom => _local ??= new Random(unchecked(Environment.TickCount * 31 + Environment.CurrentManagedThreadId));
}
