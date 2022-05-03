using LessonPlanningSystem.PlanGenerators.Models;
using LessonPlanningSystem.PlanGenerators.ValueObjects;

namespace LessonPlanningSystem.PlanGenerators.DataStructures.Timetables;

public class ScheduleTimetableDict<TKey> : Dictionary<TKey, ScheduleTimetable> where TKey : notnull
{
    public void Add(TKey key, Timetable timetable)
    {
        if (ContainsKey(key)) this[key].Add(timetable.ScheduleTime, timetable);
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
