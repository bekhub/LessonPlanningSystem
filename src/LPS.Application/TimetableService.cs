﻿#nullable enable
using AutoMapper;
using AutoMapper.QueryableExtensions;
using LPS.Application.Mapping;
using LPS.DatabaseLayer;
using LPS.DatabaseLayer.Entities;
using LPS.PlanGenerators;
using LPS.PlanGenerators.Configuration;
using LPS.PlanGenerators.DataStructures;
using LPS.PlanGenerators.Enums;
using LPS.PlanGenerators.Models;
using LPS.PlanGenerators.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Classroom = LPS.PlanGenerators.Models.Classroom;
using Course = LPS.PlanGenerators.Models.Course;
using Department = LPS.DatabaseLayer.Entities.Department;
using LessonType = LPS.PlanGenerators.Enums.LessonType;

namespace LPS.Application;

public class TimetableService
{
    private readonly TimetableV4Context _context;
    private readonly IMapper _mapper;
    private readonly PlanConfiguration _configuration;
    
    public TimetableService(TimetableV4Context context, IMapper mapper, PlanConfiguration configuration)
    {
        _context = context;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task<CoursesData> GetCoursesDataAsync(List<Department>? departments = null)
    {
        var coursesData = new CoursesData();
        var courses = _context.Courses
            .Where(x => 
                x.Semester == _configuration.Semester.ToDbValue() &&
                x.Active &&
                (departments == null || departments.Contains(x.Department)) &&
                x.UserId != 12)
            .Include(x => x.Teacher)
            .Include(x => x.Department)
                .ThenInclude(x => x.Faculty)
                    .ThenInclude(x => x.Building)
            .Include(x => x.CourseType)
            .Include(x => x.PracticeRoomType)
            .Include(x => x.TheoryRoomType)
            .Include(x => x.CourseVsRooms).ThenInclude(x => x.Classroom)
                .ThenInclude(x => x.RoomType)
            .Include(x => x.CourseVsRooms).ThenInclude(x => x.Classroom)
                .ThenInclude(x => x.Department)
            .Include(x => x.CourseVsRooms).ThenInclude(x => x.Classroom)
                .ThenInclude(x => x.Building)
            .AsSplitQuery().AsNoTracking();
        await foreach (var entity in courses.AsAsyncEnumerable()) {
            var course = _mapper.Map<Course>(entity);
            course.CourseCreated();
            coursesData.Add(course);
        }
        return coursesData;
    }

    public async Task<ClassroomsData> GetClassroomsDataAsync(IReadOnlyList<Course> courses)
    {
        var classroomsData = new ClassroomsData();
        var classrooms = _context.Classrooms
            .Include(x => x.RoomType)
            .Include(x => x.Department)
            .Include(x => x.Building)
            .AsSplitQuery().AsNoTracking()
            .ProjectTo<Classroom>(_mapper.ConfigurationProvider);
        await foreach (var classroom in classrooms.AsAsyncEnumerable()) {
            classroomsData.Add(classroom);
        }
        classroomsData.AddingEnded(courses);
        return classroomsData;
    }

    public async Task GetTimetableData(CoursesData coursesData, ClassroomsData classroomsData)
    {
        // var timetableData = new TimetableData();
        var courses = coursesData.AllCourses;
        var classrooms = classroomsData.AllClassrooms;
        var courseIds = courses.Values.Select(x => x.Id);
        var timetables = await _context.TimeTables
            .Where(x => courseIds.Contains(x.CourseId)).ToListAsync();
        foreach (var entity in timetables) {
            var course = courses[entity.CourseId];
            var lessonType = MapHelper.Parse<LessonType>(entity.LessonTypeId!.Value);
            var time = ScheduleTime.GetByWeekAndHour(MapHelper.Parse<Weekdays>(entity.TimeDayId!.Value), entity.TimeHourId!.Value);
            var classroom = classrooms[entity.ClassroomId!.Value];
            var timetable = new Timetable(course, lessonType, time, classroom);
        }
    }
    
    public async Task EnsureEnumValuesInDatabaseAsync()
    {
        foreach (var type in Enum.GetValues<LessonType>()) {
            var lessonType = await _context.LessonTypes.FindAsync(type.AsInt());
            if (lessonType != null) continue;

            _context.LessonTypes.Add(new DatabaseLayer.Entities.LessonType {
                Id = type.AsInt(),
                Name = type.ToString(),
            });
            await _context.SaveChangesAsync();
        }
        foreach (var weekdays in Enum.GetValues<Weekdays>()) {
            var timeDay = await _context.TimeDays.FindAsync(weekdays.AsInt());
            if (timeDay != null) continue;

            _context.TimeDays.Add(new TimeDay {
                Id = weekdays.AsInt(),
                OrderPosition = weekdays.AsInt(),
                Label = weekdays.ToString(),
            });
            await _context.SaveChangesAsync();
        }

        for (int i = 0; i <= 12; i++) {
            var timeHour = await _context.TimeHours.FindAsync(i + 1);
            if (timeHour != null) continue;

            _context.TimeHours.Add(new TimeHour {
                Id = i + 1,
                OrderPosition = i,
            });
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task SaveTimetableAsOriginalAsync(IEnumerable<Timetable> timetables)
    {
        await EnsureEnumValuesInDatabaseAsync();
        await DeleteCurrentSemesterTimetablesAsync();
        await _context.TimeTables.BulkInsertAsync(timetables.Select(x => {
            var timeTable = _mapper.Map<TimeTable>(x);
            timeTable.CreatedTime = DateTime.Now;
            timeTable.Semester = _configuration.Semester.ToDbValue();
            timeTable.EducationalYear = _configuration.EducationalYear;
            return timeTable;
        }));
    }

    public async Task SaveTimetableAsPreviewAsync(IEnumerable<Timetable> timetables)
    {
        await EnsureEnumValuesInDatabaseAsync();
        await TruncatePreviewTimetableAsync();
        await _context.TimeTablePreviews.BulkInsertAsync(timetables.Select(x => {
            var timeTable = _mapper.Map<TimeTablePreview>(x);
            timeTable.CreatedTime = DateTime.Now;
            timeTable.Semester = _configuration.Semester.ToDbValue();
            timeTable.EducationalYear = _configuration.EducationalYear;
            return timeTable;
        }));
    }

    public async Task DeleteCurrentSemesterTimetablesAsync()
    {
        const string sql = "delete from time_table where `educational_year` = {0} and `semester` = {1}";
        await _context.Database.ExecuteSqlRawAsync(sql, _configuration.EducationalYear,
            _configuration.Semester.ToDbValue());
    }

    public async Task TruncatePreviewTimetableAsync()
    {
        const string sql = "truncate table time_table_preview";
        await _context.Database.ExecuteSqlRawAsync(sql);
    }
}
