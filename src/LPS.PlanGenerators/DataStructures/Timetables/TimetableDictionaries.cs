using LPS.PlanGenerators.Models;
using LPS.PlanGenerators.ValueObjects;

namespace LPS.PlanGenerators.DataStructures.Timetables;

public class ScheduleTimetableDict<TKey> : Dictionary<TKey, ScheduleTimetable> where TKey : notnull
{
    public virtual void Add(TKey key, Timetable timetable)
    {
        if (ContainsKey(key)) {
            //Todo: fix this
            if (!this[key].TryAdd(timetable.ScheduleTime, timetable))
                Console.WriteLine($"{key} Id - some model is already added");
            return;
        }
        this[key] = new ScheduleTimetable {
            [timetable.ScheduleTime] = timetable,
        };
    }
}

public class ScheduleTimetablesDict<TKey> : Dictionary<TKey, ScheduleTimetables> where TKey : notnull
{
    public void Add(TKey key, Timetable timetable)
    {
        if (ContainsKey(key)) {
            var timetablesByTime = this[key];
            if (timetablesByTime.ContainsKey(timetable.ScheduleTime))
                timetablesByTime[timetable.ScheduleTime].Add(timetable);
            else timetablesByTime[timetable.ScheduleTime] = new List<Timetable> { 
                timetable,
            };
            return;
        }

        this[key] = new ScheduleTimetables {
            [timetable.ScheduleTime] = new() {
                timetable,
            },
        };
    }
}

public class ScheduleTimetable : Dictionary<ScheduleTime, Timetable> { }
public class ScheduleTimetables : Dictionary<ScheduleTime, List<Timetable>> { }
