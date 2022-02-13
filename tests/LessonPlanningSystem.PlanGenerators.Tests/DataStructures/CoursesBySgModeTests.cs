using System.Collections.Generic;
using System.Linq;
using AutoBogus;
using LessonPlanningSystem.PlanGenerators.DataStructures;
using LessonPlanningSystem.PlanGenerators.Enums;
using LessonPlanningSystem.PlanGenerators.Models;
using Xunit;

namespace LessonPlanningSystem.PlanGenerators.Tests.DataStructures;

public class CoursesBySgModeTests
{
    [Fact]
    public void CheckSubgroupMode5N6Add()
    {
        var courseMode5 = new Course {
            Id = 1,
            SubgroupMode = SubgroupMode.Mode5,
        };
        var courseMode6 = new Course {
            Id = 2,
            SubgroupMode = SubgroupMode.Mode6,
        };
        var coursesBySgMode = new CoursesBySgMode();
        coursesBySgMode.AddBySubgroupMode(courseMode5);
        Assert.Equal(1, coursesBySgMode.SubgroupMode5N6Courses.Count);
        coursesBySgMode.AddBySubgroupMode(courseMode6);
        Assert.Equal(2, coursesBySgMode.SubgroupMode5N6Courses.Count);
        Assert.Empty(coursesBySgMode.SubgroupMode4Courses);
        Assert.Empty(coursesBySgMode.SubgroupMode3Courses);
        Assert.Empty(coursesBySgMode.SpecialCourses);
        Assert.Empty(coursesBySgMode.NormalCourses);
    }
    
    [Fact]
    public void CheckSubgroupMode4Add()
    {
        var course = new Course {
            Id = 1,
            SubgroupMode = SubgroupMode.Mode4,
        };
        var coursesBySgMode = new CoursesBySgMode();
        coursesBySgMode.AddBySubgroupMode(course);
        Assert.Equal(1, coursesBySgMode.SubgroupMode4Courses.Count);
        Assert.Empty(coursesBySgMode.SubgroupMode5N6Courses);
        Assert.Empty(coursesBySgMode.SubgroupMode3Courses);
        Assert.Empty(coursesBySgMode.SpecialCourses);
        Assert.Empty(coursesBySgMode.NormalCourses);
    }
    
    [Fact]
    public void CheckSubgroupMode3Add()
    {
        var course = new Course {
            Id = 1,
            SubgroupMode = SubgroupMode.Mode3,
        };
        var coursesBySgMode = new CoursesBySgMode();
        coursesBySgMode.AddBySubgroupMode(course);
        Assert.Equal(1, coursesBySgMode.SubgroupMode3Courses.Count);
        Assert.Empty(coursesBySgMode.SubgroupMode5N6Courses);
        Assert.Empty(coursesBySgMode.SubgroupMode4Courses);
        Assert.Empty(coursesBySgMode.SpecialCourses);
        Assert.Empty(coursesBySgMode.NormalCourses);
    }

    [Fact]
    public void CheckSpecialCoursesAdd()
    {
        var courseVsRoomsFaker = new AutoFaker<CourseVsRoom>().Configure(builder => builder
            .WithSkip<Course>()
            .WithSkip<Classroom>());
        var courses = new Course[] { 
            new() { Id = 1, SubgroupMode = SubgroupMode.Mode0, CourseVsRooms = courseVsRoomsFaker.Generate(1) },
            new() { Id = 2, SubgroupMode = SubgroupMode.Mode1, CourseVsRooms = courseVsRoomsFaker.Generate(2) },
            new() { Id = 3, SubgroupMode = SubgroupMode.Mode2, CourseVsRooms = courseVsRoomsFaker.Generate(3) },
        };
        var coursesBySgMode = new CoursesBySgMode();
        foreach (var course in courses) {
            coursesBySgMode.AddBySubgroupMode(course);
        }
        Assert.Equal(3, coursesBySgMode.SpecialCourses.Count);
        Assert.Empty(coursesBySgMode.SubgroupMode5N6Courses);
        Assert.Empty(coursesBySgMode.SubgroupMode4Courses);
        Assert.Empty(coursesBySgMode.SubgroupMode3Courses);
        Assert.Empty(coursesBySgMode.NormalCourses);
    }
        
    [Fact]
    public void CheckNormalCoursesAdd()
    {
        var courses = new Course[] { 
            new() { Id = 1, SubgroupMode = SubgroupMode.Mode0, CourseVsRooms = null },
            new() { Id = 2, SubgroupMode = SubgroupMode.Mode1, CourseVsRooms = new List<CourseVsRoom>() },
            new() { Id = 3, SubgroupMode = SubgroupMode.Mode2, CourseVsRooms = new List<CourseVsRoom>() },
        };
        var coursesBySgMode = new CoursesBySgMode();
        foreach (var course in courses) {
            coursesBySgMode.AddBySubgroupMode(course);
        }
        Assert.Equal(3, coursesBySgMode.NormalCourses.Count);
        Assert.Empty(coursesBySgMode.SubgroupMode5N6Courses);
        Assert.Empty(coursesBySgMode.SubgroupMode4Courses);
        Assert.Empty(coursesBySgMode.SubgroupMode3Courses);
        Assert.Empty(coursesBySgMode.SpecialCourses);
    }

    [Fact]
    public void CheckGenerateRandomizedCoursesList()
    {
        var courseVsRoomsFaker = new AutoFaker<CourseVsRoom>().Configure(builder => builder
            .WithSkip<Course>()
            .WithSkip<Classroom>());
        var courseFaker = new AutoFaker<Course>().Configure(builder => builder
                .WithSkip<Teacher>()
                .WithSkip<Faculty>())
            .RuleFor(course => course.CourseVsRooms, faker => courseVsRoomsFaker.Generate(faker.Random.Int(0, 2)))
            .RuleFor(course => course.Id, faker => faker.Random.Int(1, 300));
        var coursesBySgMode = new CoursesBySgMode();
        foreach (var course in courseFaker.Generate(200)) {
            coursesBySgMode.AddBySubgroupMode(course);
        }

        var randomizedCoursesList = coursesBySgMode.GenerateRandomizedCoursesList();
        var initialCoursesList = coursesBySgMode.SubgroupMode5N6Courses.ToList();
        initialCoursesList.AddRange(coursesBySgMode.SubgroupMode4Courses);
        initialCoursesList.AddRange(coursesBySgMode.SubgroupMode3Courses);
        initialCoursesList.AddRange(coursesBySgMode.SpecialCourses);
        initialCoursesList.AddRange(coursesBySgMode.NormalCourses);
        
        Assert.Equal(initialCoursesList.Count, randomizedCoursesList.Count);
        Assert.NotEqual(initialCoursesList, randomizedCoursesList);
    }
}
