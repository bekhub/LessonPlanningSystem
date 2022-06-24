using LPS.PlanGenerators.Configuration;
using LPS.PlanGenerators.DataStructures;

namespace LPS.PlanGenerators;

public class ServiceProvider
{
    public PlanConfiguration PlanConfiguration { get; }
    public CoursesData CoursesData { get; }
    public ClassroomsData ClassroomsData { get; }
    public ExistingTimetable ExistingTimetable { get; }
    
    public ServiceProvider(PlanConfiguration planConfiguration, CoursesData coursesData, ClassroomsData classroomsData,
        ExistingTimetable existingTimetable)
    {
        PlanConfiguration = planConfiguration;
        CoursesData = coursesData;
        ClassroomsData = classroomsData;
        ExistingTimetable = existingTimetable;
    }

    public TimetableData GetNewTimetableData() => new(this, ExistingTimetable);
}
