#nullable enable
using LPS.PlanGenerators.DataStructures.Extensions;
using LPS.PlanGenerators.DataStructures.Timetables;
using LPS.PlanGenerators.Enums;
using LPS.PlanGenerators.Models;
using LPS.PlanGenerators.ValueObjects;

namespace LPS.PlanGenerators.DataStructures;

public sealed class TimetableData
{
    private readonly ClassroomsData _classroomsData;
    private readonly IReadOnlyList<Course> _allCourses;
    private readonly List<Timetable> _timetables;
    private readonly List<Timetable> _timetablesForRemoteCourses;
    private readonly ExistingTimetable _existingTimetable;
    
    /// <summary>
    /// Timetables by course id
    /// </summary>
    public (CoursesTimetable Current, CoursesTimetable Existing) CoursesTimetable => 
        (_coursesTimetable, _existingTimetable.CoursesTimetable);
    private readonly CoursesTimetable _coursesTimetable;
    /// <summary>
    /// Timetables by classroom id. There may be two classrooms at the same time
    /// </summary>
    public (ClassroomsTimetable Current, ClassroomsTimetable Existing) ClassroomsTimetable =>
        (_classroomsTimetable, _existingTimetable.ClassroomsTimetable);
    private readonly ClassroomsTimetable _classroomsTimetable;
    /// <summary>
    /// Timetables by teacher code. Teacher can be in only one room at the same time
    /// </summary>
    public (TeachersTimetable Current, TeachersTimetable Existing) TeachersTimetable =>
        (_teachersTimetable, _existingTimetable.TeachersTimetable);
    private readonly TeachersTimetable _teachersTimetable;
    /// <summary>
    /// Timetables by students(department id and grade year). Students can be in multiple rooms at the same time
    /// </summary>
    public (StudentsTimetable Current, StudentsTimetable Existing) StudentsTimetable =>
        (_studentsTimetable, _existingTimetable.StudentsTimetable);
    private readonly StudentsTimetable _studentsTimetable;
    /// <summary>
    /// New generated timetables
    /// </summary>
    public IReadOnlyList<Timetable> GeneratedTimetables => _timetables.Concat(_timetablesForRemoteCourses).ToList();
    /// <summary>
    /// All timetables
    /// </summary>
    public IReadOnlyList<Timetable> AllTimetables => GeneratedTimetables.Concat(_existingTimetable.Timetables).ToList();

    public TimetableData(GeneratorServiceProvider provider, ExistingTimetable existingTimetable)
    {
        _existingTimetable = existingTimetable;
        _classroomsData = provider.ClassroomsData;
        _allCourses = provider.CoursesData.AllCourseList;
        _coursesTimetable = new CoursesTimetable(provider.CoursesData);
        _classroomsTimetable = new ClassroomsTimetable(provider.ClassroomsData);
        _teachersTimetable = new TeachersTimetable();
        _studentsTimetable = new StudentsTimetable();
        _timetables = new List<Timetable>();
        _timetablesForRemoteCourses = new List<Timetable>();
    }

    /// <summary>
    /// Add timetable for single course
    /// </summary>
    public void AddTimetable(Course course, LessonType lessonType, ScheduleTimeRange timeRange, 
        (Classroom, Classroom?) rooms)
    {
        foreach (var time in timeRange.GetScheduleTimes()) {
            var (room, additional) = rooms;
            AddTimetable(new Timetable(course, lessonType, time, room, additional));
        }
    }
    
    public void AddTimetableForRemoteCourse(Course course, LessonType lessonType, ScheduleTimeRange timeRange, int roomId)
    {
        var room = _classroomsData.AllClassrooms[roomId];
        foreach (var time in timeRange.GetScheduleTimes()) {
            _timetablesForRemoteCourses.Add(new Timetable(course, lessonType, time, room));
        }
    }
    
    /// <summary>
    /// Add timetable for multiple courses
    /// </summary>
    public void AddTimetable(IReadOnlyList<Course> courses, LessonType lessonType, ScheduleTimeRange timeRange, 
        (Classroom, Classroom?) rooms)
    {
        // Todo: Almost old logic
        foreach (var time in timeRange.GetScheduleTimes()) {
            var firstCourse = courses[0];
            var (room, additional) = rooms;
            
            var timetable = new Timetable(firstCourse, lessonType, time, room, additional);
            _teachersTimetable.Add(firstCourse.Teacher.Code, timetable);
            _classroomsTimetable.Add(room.Id, timetable);
            if (additional != null) _classroomsTimetable.Add(additional.Id, timetable);
            _timetables.Add(timetable);
            foreach (var course in courses) {
                timetable = new Timetable(course, lessonType, time, room);
                _coursesTimetable.Add(course.Id, timetable);
                _studentsTimetable.Add((course.Department.Id, course.GradeYear), timetable);
                if (course.Id != firstCourse.Id) _timetables.Add(timetable);
            }
        }
    }
    
    /// <summary>
    /// Add timetable
    /// </summary>
    public void AddTimetable(Timetable timetable)
    {
        _coursesTimetable.Add(timetable.Course.Id, timetable);
        _teachersTimetable.Add(timetable.Course.Teacher.Code, timetable);
        _studentsTimetable.Add((timetable.Course.Department.Id, timetable.Course.GradeYear), timetable);
        _classroomsTimetable.Add(timetable.Classroom.Id, timetable);
        if (timetable.AdditionalClassroom != null) _classroomsTimetable.Add(timetable.AdditionalClassroom.Id, timetable);
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
            : GetFreeRooms(rooms, timeRange);
    }
    
    /// <summary>
    /// Returns free rooms by courses, lesson type, time range, and round
    /// </summary>
    public IReadOnlyList<Classroom> GetFreeRoomsByCourses(IReadOnlyList<Course> courses, LessonType lessonType, 
        ScheduleTimeRange timeRange, Round round)
    {
        var rooms = GetFreeRooms(courses
            .SelectMany(x => x.GetRoomsForSpecialCourses(lessonType)).Distinct(), timeRange);
        if (rooms.Count != 0) return rooms;

        var coursesClassrooms = courses
            .Select(x => _classroomsData.GetClassroomsByCourse(x, lessonType, round)).ToList();
        if (coursesClassrooms.Any(x => x.Count == 0)) return rooms;
        rooms = GetFreeRooms(coursesClassrooms.SelectMany(x => x).Distinct(), timeRange);

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
