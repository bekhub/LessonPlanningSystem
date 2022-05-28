﻿using System.Diagnostics;
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
var configuration = new PlanConfiguration {
    IncludeGeneralMandatoryCourses = false,
    IncludeRemoteEducationCourses = false,
    Semester = Semester.Autumn,
    EducationalYear = "2021-2022",
    NumberOfVariants = 100,
    UnpositionedLessonsCoefficient = 100,
    SeparatedLessonsCoefficient = 1,
    MaxTeachingHoursCoefficient = 1,
    MaxNumberOfThreads = null,
};
service.AddSingleton(configuration);
await using var provider = service.BuildServiceProvider();
var timetableService = provider.GetRequiredService<TimetableService>();
var stopwatch = new Stopwatch();
stopwatch.Start();
var coursesData = await timetableService.GetCoursesDataAsync();
var classroomData = await timetableService.GetClassroomsDataAsync(coursesData.AllCourses.Values.ToList());
stopwatch.Stop();
Console.WriteLine("Reading data: " + stopwatch.Elapsed);
var planGenerator = new BestPlanGenerator(configuration, coursesData, classroomData);
stopwatch.Restart();
var bestLessonPlan = await planGenerator.GenerateBestLessonPlanAsync();
stopwatch.Stop();
Console.WriteLine("Generating lesson plan: " + stopwatch.Elapsed);
stopwatch.Restart();
await timetableService.SaveTimetableAsOriginalAsync(bestLessonPlan.Timetables);
stopwatch.Stop();
Console.WriteLine("Saving lesson plan: " + stopwatch.Elapsed);