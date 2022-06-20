using System.Collections.Generic;
using System.Linq;
using AutoBogus;
using FluentAssertions;
using LPS.PlanGenerators.DataStructures;
using LPS.PlanGenerators.Enums;
using LPS.PlanGenerators.Models;
using Xunit;

namespace LPS.PlanGenerators.Tests.DataStructures;

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
        coursesBySgMode.SubgroupMode5N6Courses.Count.Should().Be(1);
        coursesBySgMode.AddBySubgroupMode(courseMode6);
        coursesBySgMode.SubgroupMode5N6Courses.Count.Should().Be(2);
        coursesBySgMode.SubgroupMode4Courses.Should().BeEmpty();
        coursesBySgMode.SubgroupMode3Courses.Should().BeEmpty();
        coursesBySgMode.SpecialCourses.Should().BeEmpty();
        coursesBySgMode.NormalCourses.Should().BeEmpty();
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
        coursesBySgMode.SubgroupMode4Courses.Count.Should().Be(1);
        coursesBySgMode.SubgroupMode5N6Courses.Should().BeEmpty();
        coursesBySgMode.SubgroupMode3Courses.Should().BeEmpty();
        coursesBySgMode.SpecialCourses.Should().BeEmpty();
        coursesBySgMode.NormalCourses.Should().BeEmpty();
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
        coursesBySgMode.SubgroupMode3Courses.Count.Should().Be(1);
        coursesBySgMode.SubgroupMode5N6Courses.Should().BeEmpty();
        coursesBySgMode.SubgroupMode4Courses.Should().BeEmpty();
        coursesBySgMode.SpecialCourses.Should().BeEmpty();
        coursesBySgMode.NormalCourses.Should().BeEmpty();
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
        coursesBySgMode.SpecialCourses.Count.Should().Be(3);
        coursesBySgMode.SubgroupMode5N6Courses.Should().BeEmpty();
        coursesBySgMode.SubgroupMode4Courses.Should().BeEmpty();
        coursesBySgMode.SubgroupMode3Courses.Should().BeEmpty();
        coursesBySgMode.NormalCourses.Should().BeEmpty();
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
        coursesBySgMode.NormalCourses.Count.Should().Be(3);
        coursesBySgMode.SubgroupMode5N6Courses.Should().BeEmpty();
        coursesBySgMode.SubgroupMode4Courses.Should().BeEmpty();
        coursesBySgMode.SubgroupMode3Courses.Should().BeEmpty();
        coursesBySgMode.SpecialCourses.Should().BeEmpty();
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
        
        initialCoursesList.Count.Should().Be(randomizedCoursesList.Count);
        initialCoursesList.Should().NotEqual(randomizedCoursesList);
    }
}
