#nullable enable
using LPS.PlanGenerators.DataStructures.Timetables;
using LPS.PlanGenerators.Enums;
using LPS.PlanGenerators.Models;
using LPS.PlanGenerators.ValueObjects;

namespace LPS.PlanGenerators.DataStructures;

public class TimetableData
{
    private readonly ClassroomsData _classroomsData;
    private readonly IReadOnlyList<Course> _allCourses;
    private readonly List<Timetable> _timetables;
    
    /// <summary>
    /// Timetables by course id
    /// </summary>
    public readonly CoursesTimetable CoursesTimetable;
    /// <summary>
    /// Timetables by classroom id. There may be two classrooms at the same time
    /// </summary>
    public readonly ClassroomsTimetable ClassroomsTimetable;
    /// <summary>
    /// Timetables by teacher code. Teacher can be in only one room at the same time
    /// </summary>
    public readonly TeachersTimetable TeachersTimetable;
    /// <summary>
    /// Timetables by students(department id and grade year). Students can be in multiple rooms at the same time
    /// </summary>
    public readonly StudentsTimetable StudentsTimetable;
    /// <summary>
    /// All timetables
    /// </summary>
    public IReadOnlyList<Timetable> Timetables => _timetables;

    public TimetableData(ServiceProvider provider)
    {
        CoursesTimetable = new CoursesTimetable(provider);
        ClassroomsTimetable = new ClassroomsTimetable(provider);
        TeachersTimetable = new TeachersTimetable();
        StudentsTimetable = new StudentsTimetable();
        _timetables = new List<Timetable>();
        _classroomsData = provider.ClassroomsData;
        _allCourses = provider.CoursesData.AllCourses.Values.ToList();
    }

    /// <summary>
    /// 
    /// </summary>
    public void AddTimetable(Course course, LessonType lessonType, ScheduleTimeRange timeRange, 
        IReadOnlyList<Classroom> rooms)
    {
        foreach (var time in timeRange.GetScheduleTimes()) {
            foreach (var room in rooms) {
                AddTimetable(new Timetable(course, lessonType, time, room));
            }
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    public void AddTimetable(IReadOnlyList<Course> courses, LessonType lessonType, ScheduleTimeRange timeRange, 
        IReadOnlyList<Classroom> rooms)
    {
        // Todo: Almost old logic
        foreach (var time in timeRange.GetScheduleTimes()) {
            var firstCourse = courses[0];
            var firstRoom = rooms[0];
            
            var timetable = new Timetable(firstCourse, lessonType, time, firstRoom);
            TeachersTimetable.Add(firstCourse.Teacher.Code, timetable);
            
            foreach (var room in rooms) {
                timetable = new Timetable(firstCourse, lessonType, time, room);
                if (room == null) throw new Exception("What the hell!?");
                ClassroomsTimetable.Add(room.Id, timetable);
                _timetables.Add(timetable);
            }
            foreach (var course in courses) {
                timetable = new Timetable(course, lessonType, time, firstRoom);
                CoursesTimetable.Add(course.Id, timetable);
                StudentsTimetable.Add((course.Department.Id, course.GradeYear), timetable);
                if (course.Id != firstCourse.Id) _timetables.Add(timetable);
            }
        }
    }
    
    /// <summary>
    /// Add timetable
    /// </summary>
    public void AddTimetable(Timetable timetable)
    {
        CoursesTimetable.Add(timetable.Course.Id, timetable);
        TeachersTimetable.Add(timetable.Course.Teacher.Code, timetable);
        StudentsTimetable.Add((timetable.Course.Department.Id, timetable.Course.GradeYear), timetable);
        ClassroomsTimetable.Add(timetable.Classroom.Id, timetable);
        _timetables.Add(timetable);
    }

    /// <summary>
    /// 
    /// </summary>
    public IReadOnlyList<Classroom> GetRoomsByCourse(Course course, LessonType lessonType, Round round)
    {
        var roomsForSpecialCourses = course.GetRoomsForSpecialCourses(lessonType);
        var rooms = roomsForSpecialCourses.Any() ? roomsForSpecialCourses 
            : _classroomsData.GetClassroomsByCourse(course, lessonType, round);
        return rooms.ToList();
    }
    
    /// <summary>
    /// 
    /// </summary>
    public IReadOnlyList<Classroom> GetFreeRooms(IEnumerable<Classroom> rooms, ScheduleTimeRange timeRange)
    {
        return rooms.Where(x => ClassroomsTimetable.RoomIsFree(x, timeRange)).ToList();
    }

    /// <summary>
    /// 
    /// </summary>
    public IReadOnlyList<Classroom> GetFreeRoomsByCourse(Course course, LessonType lessonType, 
        ScheduleTimeRange timeRange, Round round)
    {
        var roomsForSpecialCourses = course.GetRoomsForSpecialCourses(lessonType);
        var rooms = roomsForSpecialCourses.Any() ? roomsForSpecialCourses 
            : _classroomsData.GetClassroomsByCourse(course, lessonType, round);
        return rooms.Any(x => x == null) ? new List<Classroom>() 
            : rooms.Where(x => ClassroomsTimetable.RoomIsFree(x, timeRange)).ToList();
    }
    
    /// <summary>
    /// 
    /// </summary>
    public IReadOnlyList<Classroom> GetFreeRoomsByCourses(IReadOnlyList<Course> courses, LessonType lessonType, 
        ScheduleTimeRange timeRange, Round round)
    {
        var rooms = courses
            .SelectMany(x => x.GetRoomsForSpecialCourses(lessonType)).Distinct()
            .Where(x => ClassroomsTimetable.RoomIsFree(x, timeRange)).ToList();
        if (rooms.Count != 0) return rooms;

        var coursesClassrooms = courses
            .Select(x => _classroomsData.GetClassroomsByCourse(x, lessonType, round)).ToList();
        if (coursesClassrooms.Any(x => x.Count == 0)) return rooms;
        rooms = coursesClassrooms.SelectMany(x => x).Distinct()
            .Where(x => ClassroomsTimetable.RoomIsFree(x, timeRange)).ToList();

        return rooms;
    }

    /// <summary>
    /// Checks if course, students and teacher are free at the given time
    /// </summary>
    public bool ScheduleTimeRangeIsFree(Course course, ScheduleTimeRange timeRange, Round round)
    {
        foreach (var currentTime in timeRange.GetScheduleTimes()) {
            //check if the course free at that time (may be the practice lesson at the same time but in the other room)
            if (!CoursesTimetable.CourseIsFree(course.Id, currentTime)) return false;

            // check if the students are free at the given time
            if (!StudentsTimetable.StudentsAreFree(course, currentTime, round)) return false;

            // check if the teacher is free at the given time
            if (!TeachersTimetable.TeacherIsFree(course.Teacher, currentTime)) return false;
        }

        return true;
    }

    /// <summary>
    /// Returns remaining hours for course by lesson type
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public int RemainingHoursByLessonType(Course course, LessonType lessonType) => lessonType switch {
        // If there is only 1 hour for theory lesson, may be it is better to put it together with practice lessons
        LessonType.Theory when CoursesTimetable.UnpositionedTheoryHours(course) == 1 && 
                               course.PracticeHours is not 0 and < 3 && 
                               course.TheoryRoomType == course.PracticeRoomType => 0,
        LessonType.Practice when CoursesTimetable.UnpositionedTheoryHours(course) == 1 && 
                                 course.PracticeHours is not 0 and < 3 && 
                                 course.TheoryRoomType == course.PracticeRoomType => 
            CoursesTimetable.UnpositionedPracticeHours(course) + 1,
        
        LessonType.Theory => CoursesTimetable.UnpositionedTheoryHours(course),
        LessonType.Practice => CoursesTimetable.UnpositionedPracticeHours(course),
        _ => throw new ArgumentOutOfRangeException(nameof(lessonType), lessonType, "Not handled all lesson types!"),
    };
    
    public IReadOnlyList<Course> MergeCoursesByCourse(Course course)
    {
        return _allCourses.Where(x =>
            x.Code == course.Code &&
            x.Department.Faculty.Id == course.Department.Faculty.Id &&
            x.SubgroupMode == course.SubgroupMode).ToList();
    }
}
