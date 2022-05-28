namespace LessonPlanningSystem.Utils;

public static class HelperExtensions
{
    /// <summary>
    /// <a href="https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle">Fisher–Yates shuffle</a> algorithm.
    /// Shuffles a copy of a given list. The method is thread-safe.
    /// Taken from <a href="https://stackoverflow.com/a/1262619/16976426">here</a>
    /// </summary>
    /// <param name="items">list of items to shuffle</param>
    /// <typeparam name="TItem">generic item type</typeparam>
    /// <returns>Shuffled copy of a given list</returns>
    public static List<TItem> ShuffleOrdinary<TItem>(this IReadOnlyCollection<TItem> items)
    {
        var itemsCopy = items.ToList();
        int currIdx = itemsCopy.Count;
        while (currIdx > 1) {
            currIdx--;
            int rndNmr = Random.Shared.Next(currIdx + 1);
            (itemsCopy[rndNmr], itemsCopy[currIdx]) = (itemsCopy[currIdx], itemsCopy[rndNmr]);
        }

        return itemsCopy;
    }
    
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? collection)
    {
        return collection == null || !collection.Any();
    }
    
    /// <summary>
    /// Shuffles a copy of a given list. The method is thread-safe.
    /// Generates a full sequence of random indexes of list size.
    /// </summary>
    /// <param name="items">list of items to shuffle</param>
    /// <typeparam name="TItem">generic item type</typeparam>
    /// <returns>Shuffled copy of a given list</returns>
    public static List<TItem> FullShuffle<TItem>(this IReadOnlyCollection<TItem> items)
    {
        var set = new HashSet<int>();
        var itemsCopy = items.ToList();
        var n = itemsCopy.Count;
        while (set.Count != n) {
            var rndNmr = Random.Shared.Next(n);
            var currIdx = n - set.Count - 1;
            if (set.Add(rndNmr)) (itemsCopy[rndNmr], itemsCopy[currIdx]) = (itemsCopy[currIdx], itemsCopy[rndNmr]);
        }

        return itemsCopy;
    }
}
