using System.Diagnostics;
using LessonPlanningSystem.Application;
using LessonPlanningSystem.DatabaseLayer;
using LessonPlanningSystem.PlanGenerators.Configuration;
using LessonPlanningSystem.PlanGenerators.Enums;
using Microsoft.Extensions.DependencyInjection;

var service = new ServiceCollection();
var connectionString = "server=localhost;database=timetable_v4;user=root;password=root";
var version = "8.0.28";
service.AddTimetableDb(connectionString, Version.Parse(version));
service.AddApplicationDependencies();
await using var provider = service.BuildServiceProvider();
var timetableService = provider.GetRequiredService<TimetableService>();
var configuration = new PlanConfiguration {
    IncludeGeneralMandatoryCourses = false,
    IncludeRemoteEducationCourses = false,
    Semester = Semester.Spring,
    NumberOfVariants = 1,
    UnpositionedLessonsCoefficient = 100,
    SeparatedLessonsCoefficient = 10,
    MaxTeachingHoursCoefficient = 1,
    MaxNumberOfThreads = null,
};
var planGenerator = new BestPlanGenerator(configuration, timetableService);
var stopwatch = new Stopwatch();
stopwatch.Start();
await planGenerator.ReadRequiredDataAsync();
stopwatch.Stop();
Console.WriteLine("Reading data: " + stopwatch.Elapsed);
stopwatch.Restart();
var timetableData = planGenerator.GenerateBestLessonPlan();
stopwatch.Stop();
Console.WriteLine("Generating lesson plan: " + stopwatch.Elapsed);
