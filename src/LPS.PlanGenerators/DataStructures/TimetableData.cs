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
    private readonly ExistingTimetable _existingTimetable;
    
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

    public TimetableData(ServiceProvider provider, ExistingTimetable existingTimetable)
    {
        _existingTimetable = existingTimetable;
        _classroomsData = provider.ClassroomsData;
        _allCourses = provider.CoursesData.AllCourseList;
        CoursesTimetable = new CoursesTimetable(provider.CoursesData);
        ClassroomsTimetable = new ClassroomsTimetable(provider.ClassroomsData);
        TeachersTimetable = new TeachersTimetable();
        StudentsTimetable = new StudentsTimetable();
        _timetables = new List<Timetable>();
    }

    /// <summary>
    /// Add timetable for single course
    /// </summary>
    public void AddTimetable(Course course, LessonType lessonType, ScheduleTimeRange timeRange, 
        IReadOnlyList<Classroom> rooms)
    {
        //Todo: This won't work, there can be only one same course and time in CoursesTimetable. Should be changed
        foreach (var time in timeRange.GetScheduleTimes()) {
            foreach (var room in rooms) {
                AddTimetable(new Timetable(course, lessonType, time, room));
            }
        }
    }
    
    /// <summary>
    /// Add timetable for multiple courses
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
    /// Returns proper rooms for course by lesson type and round
    /// </summary>
    public IReadOnlyList<Classroom> GetRoomsByCourse(Course course, LessonType lessonType, Round round)
    {
        var roomsForSpecialCourses = course.GetRoomsForSpecialCourses(lessonType);
        var rooms = roomsForSpecialCourses.Any() ? roomsForSpecialCourses 
            : _classroomsData.GetClassroomsByCourse(course, lessonType, round);
        return rooms.ToList();
    }
    
    /// <summary>
    /// Returns free rooms from given rooms by time range
    /// </summary>
    public IReadOnlyList<Classroom> GetFreeRooms(IEnumerable<Classroom> rooms, ScheduleTimeRange timeRange)
    {
        return rooms.Where(x => ClassroomsTimetable.RoomIsFree(x, timeRange)).ToList();
    }

    /// <summary>
    /// Returns free rooms by course, lesson type, time range, and round
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
    /// Returns free rooms by courses, lesson type, time range, and round
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
    
    /// <summary>
    /// Merges courses by course code, faculty, and subgroup merge mode
    /// </summary>
    public IReadOnlyList<Course> MergeCoursesByCourse(Course course)
    {
        return _allCourses.Where(x =>
            x.Code == course.Code &&
            x.Department.Faculty.Id == course.Department.Faculty.Id &&
            x.SubgroupMode == course.SubgroupMode).ToList();
    }
}
