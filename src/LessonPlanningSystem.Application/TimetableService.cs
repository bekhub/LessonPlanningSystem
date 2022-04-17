using AutoMapper;
using AutoMapper.QueryableExtensions;
using LessonPlanningSystem.Application.Mapping;
using LessonPlanningSystem.DatabaseLayer;
using LessonPlanningSystem.PlanGenerators.DataStructures;
using LessonPlanningSystem.PlanGenerators.Enums;
using LessonPlanningSystem.PlanGenerators.Models;
using Microsoft.EntityFrameworkCore;

namespace LessonPlanningSystem.Application;

public class TimetableService
{
    private readonly TimetableV4Context _context;
    private readonly IMapper _mapper;
    
    public TimetableService(TimetableV4Context context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<CoursesData> GetCoursesDataAsync(Semester semester)
    {
        var coursesData = new CoursesData();
        var courses = _context.Courses
            .Where(x => 
                x.Semester == semester.ToDbValue() &&
                x.Active &&
                // Todo: remove null checks, why we have nulls in database?
                x.Teacher != null &&
                x.UserId != 12 &&
                x.CourseVsRooms.All(z => z.Classroom != null))
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
            .AsSplitQuery();
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
            .AsSplitQuery()
            .ProjectTo<Classroom>(_mapper.ConfigurationProvider);
        await foreach (var classroom in classrooms.AsAsyncEnumerable()) {
            classroomsData.Add(classroom);
        }
        classroomsData.AddingEnded(courses);
        return classroomsData;
    }
}
