using System.Diagnostics;
using LPS.Application;
using LPS.DatabaseLayer;
using LPS.PlanGenerators.Configuration;
using LPS.PlanGenerators.Enums;
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
    Semester = Semester.Autumn,
    NumberOfVariants = 1,
    UnpositionedLessonsCoefficient = 100,
    SeparatedLessonsCoefficient = 10,
    MaxTeachingHoursCoefficient = 1,
    MaxNumberOfThreads = null,
};
var stopwatch = new Stopwatch();
stopwatch.Start();
var coursesData = await timetableService.GetCoursesDataAsync(configuration.Semester);
var classroomData = await timetableService.GetClassroomsDataAsync(coursesData.AllCourses.Values.ToList());
stopwatch.Stop();
Console.WriteLine("Reading data: " + stopwatch.Elapsed);
var planGenerator = new BestPlanGenerator(configuration, coursesData, classroomData);
stopwatch.Restart();
var timetableData = await planGenerator.GenerateBestLessonPlanAsync();
stopwatch.Stop();
Console.WriteLine("Generating lesson plan: " + stopwatch.Elapsed);
