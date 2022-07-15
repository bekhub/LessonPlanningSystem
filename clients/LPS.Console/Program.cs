using System.Diagnostics;
using LPS.Application;
using LPS.DatabaseLayer;
using LPS.DatabaseLayer.Entities;
using LPS.PlanGenerators;
using LPS.PlanGenerators.Configuration;
using LPS.PlanGenerators.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var service = new ServiceCollection();
var connectionString = "server=localhost;database=timetable_v4;user=root;password=root";
var version = "8.0.28";
service.AddTimetableDb(connectionString, Version.Parse(version));
service.AddApplicationDependencies();
var configuration = new PlanConfiguration {
    IncludeGeneralMandatoryCourses = false,
    IncludeRemoteEducationCourses = false,
    Semester = Semester.Autumn,
    EducationalYear = "2021-2022",
    NumberOfVariants = 1,
    UnpositionedLessonsCoefficient = 100,
    SeparatedLessonsCoefficient = 1,
    MaxTeachingHoursCoefficient = 1,
    MaxNumberOfThreads = 1,
};
service.AddSingleton(configuration);
await using var provider = service.BuildServiceProvider();
var db = provider.GetRequiredService<TimetableV4Context>();
var timetableService = provider.GetRequiredService<TimetableService>();

var stopwatch = new Stopwatch();
stopwatch.Start();
var department = await db.Departments.FirstAsync(x => x.Id == 9);
var coursesData = await timetableService.GetCoursesDataAsync(new List<Department> {department});
var classroomData = await timetableService.GetClassroomsDataAsync(coursesData.AllCourseList);
var existingTimetable = await timetableService.GetExistingTimetable(coursesData, classroomData);
stopwatch.Stop();
Console.WriteLine("Reading data: " + stopwatch.Elapsed);

var generatorProvider = new GeneratorServiceProvider(configuration, coursesData, classroomData, existingTimetable);

var planGenerator = new BestPlanGenerator(generatorProvider);
stopwatch.Restart();
var bestLessonPlan = await planGenerator.GenerateBestLessonPlanAsync();
stopwatch.Stop();
Console.WriteLine("Generating lesson plan: " + stopwatch.Elapsed);

stopwatch.Restart();
await timetableService.SaveTimetableAsPreviewAsync(bestLessonPlan);
stopwatch.Stop();
Console.WriteLine("Saving lesson plan: " + stopwatch.Elapsed);
