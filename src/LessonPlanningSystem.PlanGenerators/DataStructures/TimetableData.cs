#nullable enable
using LessonPlanningSystem.Generator.DataStructures.Timetables;
using LessonPlanningSystem.PlanGenerators.Enums;
using LessonPlanningSystem.PlanGenerators.Models;
using LessonPlanningSystem.PlanGenerators.ValueObjects;

namespace LessonPlanningSystem.PlanGenerators.DataStructures;

public class TimetableData
{
    private readonly ClassroomsData _classroomsData;
    
    /// <summary>
    /// Timetables by course id
    /// </summary>
    public readonly CoursesTimetable CoursesTimetable = new();
    /// <summary>
    /// Timetables by classroom id. There may be two classrooms at the same time
    /// </summary>
    public readonly ClassroomsTimetable ClassroomsTimetable = new();
    /// <summary>
    /// Timetables by teacher code. Teacher can be in only one room at the same time
    /// </summary>
    public readonly TeachersTimetable TeachersTimetable = new();
    /// <summary>
    /// Timetables by students(department id and grade year). Students can be in multiple rooms at the same time
    /// </summary>
    public readonly StudentsTimetable StudentsTimetable = new();

    private readonly List<Timetable> _timetables = new();

    private readonly IReadOnlyList<Course> _allCourses;
    
    public IReadOnlyList<Timetable> Timetables => _timetables;

    public TimetableData(ClassroomsData classroomsData, IReadOnlyList<Course> allCourses)
    {
        _classroomsData = classroomsData;
        _allCourses = allCourses;
    }

    public void AddTimetable(Course course, LessonType lessonType, ScheduleTimeRange timeRange, 
        IReadOnlyList<Classroom> rooms)
    {
        foreach (var time in timeRange.GetScheduleTimes()) {
            foreach (var room in rooms) {
                var timetable = new Timetable(course, lessonType, time, room);
                CoursesTimetable.Add(course.Id, timetable);
                TeachersTimetable.Add(course.Teacher.Code, timetable);
                StudentsTimetable.Add((course.Department.Id, course.GradeYear), timetable);
                ClassroomsTimetable.Add(room.Id, timetable);
                _timetables.Add(timetable);
            }
        }
    }
    
    public void AddTimetable(IReadOnlyList<Course> courses, LessonType lessonType, ScheduleTimeRange timeRange, 
        IReadOnlyList<Classroom> rooms)
    {
        // Todo: Changed logic
        foreach (var time in timeRange.GetScheduleTimes()) {
            var coursesRoomsCrossJoin = from course in courses from room in rooms 
                select (course, room);
            foreach (var (course, room) in coursesRoomsCrossJoin) {
                var timetable = new Timetable(course, lessonType, time, room);
                CoursesTimetable.Add(course.Id, timetable);
                TeachersTimetable.Add(course.Teacher.Code, timetable);
                StudentsTimetable.Add((course.Department.Id, course.GradeYear), timetable);
                ClassroomsTimetable.Add(room.Id, timetable);
                _timetables.Add(timetable);
            }
        }
    }

    /// <summary>
    /// This function calculates the total number of unpositioned lessons
    /// </summary>
    /// <returns></returns>
    public int TotalUnpositionedLessons()
    {
        return _allCourses.Sum(x => 
            CoursesTimetable.UnpositionedPracticeHours(x) + CoursesTimetable.UnpositionedTheoryHours(x));
    }

    /// <summary>
    /// This function calculates the total free hours of the rooms
    /// </summary>
    /// <returns></returns>
    public int TotalFreeHoursOfRooms()
    {
        var totalHours = ScheduleTime.GetWeekScheduleTimes().Count();
        return _classroomsData.AllClassrooms.Values.Where(x => 
            x.RoomType is not (RoomType.WithComputers or RoomType.Laboratory or RoomType.Gym))
            .Sum(x => ClassroomsTimetable.ContainsKey(x.Id)
                ? totalHours - ClassroomsTimetable[x.Id].Count
                : totalHours);
    }

    /// <summary>
    /// This function calculates the total number of unpositioned courses
    /// </summary>
    /// <returns></returns>
    public int TotalUnpositionedCourses()
    {
        return _allCourses.Count(x => 
            CoursesTimetable.UnpositionedPracticeHours(x) + CoursesTimetable.UnpositionedTheoryHours(x) > 0);
    }

    /// <summary>
    /// This function calculates the total number of separated lessons
    /// </summary>
    /// <returns></returns>
    public int TotalSeparatedLessons() => CoursesTimetable.TotalSeparatedLessons();

    /// <summary>
    /// This function calculates the maximum number of hours to teach for a teacher without break during one day.
    /// </summary>
    /// <returns></returns>
    public int MaxTeachingHours() => TeachersTimetable.MaxTeachingHours();

    public List<Classroom>? FindFreeRoomsWithMatchedCapacity(Course course, LessonType lessonType, 
        ScheduleTimeRange timeRange, Round round, Func<int, int, bool>? capacityCheck = null)
    {
        var freeRoom = FindFreeRoomWithMatchedCapacity(course, lessonType, timeRange, round, capacityCheck);
        if (freeRoom != null) return new List<Classroom> { freeRoom };
        if (lessonType == LessonType.Practice && course.PracticeRoomType is RoomType.WithComputers or RoomType.Laboratory)
            return FindTwoFreeRoomsWithMatchedCapacity(course, lessonType, timeRange, round, capacityCheck);
        return null;
    }

    /// <summary>
    /// Finds room with capacity that is enough for the course 
    /// </summary>
    /// <param name="course"></param>
    /// <param name="lessonType"></param>
    /// <param name="timeRange"></param>
    /// <param name="round"></param>
    /// <param name="capacityCheck"></param>
    /// <returns></returns>
    public Classroom? FindFreeRoomWithMatchedCapacity(Course course, LessonType lessonType, ScheduleTimeRange timeRange, 
        Round round, Func<int, int, bool>? capacityCheck = null)
    {
        var freeRooms = GetFreeRoomsByCourse(course, lessonType, timeRange, round);
        return capacityCheck == null
            ? freeRooms.FirstOrDefault(x => course.MaxStudentsNumber <= x.Capacity + 10)
            : freeRooms.FirstOrDefault(x => capacityCheck(course.MaxStudentsNumber, x.Capacity));
    }

    /// <summary>
    /// Finds two rooms with total capacity that is enough for the course 
    /// </summary>
    /// <param name="course"></param>
    /// <param name="lessonType"></param>
    /// <param name="timeRange"></param>
    /// <param name="round"></param>
    /// <param name="capacityCheck"></param>
    /// <returns></returns>
    public List<Classroom>? FindTwoFreeRoomsWithMatchedCapacity(Course course, LessonType lessonType, 
        ScheduleTimeRange timeRange, Round round, Func<int, int, bool>? capacityCheck = null)
    {
        var freeRooms = GetFreeRoomsByCourse(course, lessonType, timeRange, round);
        // Todo: I didn't understand why we do so
        foreach (var byBuilding in freeRooms.GroupBy(x => x.Building.Id)) {
            var rooms = byBuilding.ToList();
            for (int col = 0; col < rooms.Count; col++) {
                for (int row = 0; row < col; row++) {
                    var (rowRoom, colRoom) = (rooms[row], rooms[col]);
                    int currentCapacity = rowRoom.Capacity + colRoom.Capacity;
                    if (capacityCheck == null && course.MaxStudentsNumber <= currentCapacity + 10)
                        return new List<Classroom> { rowRoom, colRoom };
                    if (capacityCheck != null && capacityCheck(course.MaxStudentsNumber, currentCapacity)) 
                        return new List<Classroom> { rowRoom, colRoom };
                }
            }
        }
        return null;
    }
    
    private IEnumerable<Classroom> GetFreeRoomsByCourse(Course course, LessonType lessonType, 
        ScheduleTimeRange timeRange, Round round)
    {
        var roomsForSpecialCourses = course.GetRoomsForSpecialCourses(lessonType);
        var rooms = roomsForSpecialCourses.Any() ? roomsForSpecialCourses 
            : _classroomsData.GetClassroomsByCourse(course, lessonType, round);
        return rooms.Any(x => x == null) ? new List<Classroom>() 
            : rooms.Where(x => ClassroomsTimetable.RoomIsFree(x, timeRange));
    }
    
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
    /// <param name="course"></param>
    /// <param name="timeRange"></param>
    /// <param name="round"></param>
    /// <returns></returns>
    public bool ScheduleTimeRangeIsFree(Course course, ScheduleTimeRange timeRange, Round round)
    {
        foreach (var currentTime in timeRange.GetScheduleTimes()) {
            //check if the course free at that time (may be the uygulama lesson at the same time but in the other room)
            if (!CoursesTimetable.CourseIsFree(course.Id, currentTime)) return false;

            // check if the students are free at the given time
            // Todo: keep <round> logic in one place
            if (!StudentsTimetable.StudentsAreFree(course, currentTime, round)) return false;

            // check if the teacher is free at the given time
            if (!TeachersTimetable.TeacherIsFree(course.Teacher, currentTime)) return false;
        }

        return true;
    }

    /// <summary>
    /// Returns remaining hours for course by lesson type
    /// </summary>
    /// <param name="course"></param>
    /// <param name="lessonType"></param>
    /// <returns></returns>
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
