using LPS.PlanGenerators.Configuration;
using LPS.PlanGenerators.DataStructures;

namespace LPS.PlanGenerators;

public class ServiceProvider
{
    public PlanConfiguration PlanConfiguration { get; }
    public CoursesData CoursesData { get; }
    public ClassroomsData ClassroomsData { get; }
    
    public ServiceProvider(PlanConfiguration planConfiguration, CoursesData coursesData, ClassroomsData classroomsData)
    {
        PlanConfiguration = planConfiguration;
        CoursesData = coursesData;
        ClassroomsData = classroomsData;
    }

    public TimetableData GetNewTimetableData() => new(this);
}
