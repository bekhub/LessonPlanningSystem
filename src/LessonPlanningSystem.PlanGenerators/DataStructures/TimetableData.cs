#nullable enable
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
    private readonly TimetableByTimeByKey<int> _coursesTimetable = new();
    /// <summary>
    /// Timetables by classroom id. There may be two classrooms at the same time
    /// </summary>
    private readonly TimetablesByTimeByKey<int> _classroomsTimetable = new();
    /// <summary>
    /// Timetables by teacher code. Teacher can be in only one room at the same time
    /// </summary>
    private readonly TimetableByTimeByKey<int> _teachersTimetable = new();
    /// <summary>
    /// Timetables by students(department id and grade year). Students can be in multiple rooms at the same time
    /// </summary>
    private readonly TimetablesByTimeByKey<(int DepartmentId, GradeYear gradeYear)> _studentsTimetable = new();

    public TimetableData(ClassroomsData classroomsData) {
        _classroomsData = classroomsData;
    }

    public void AddTimetable(Timetable timetable)
    {
        var course = timetable.Course;
        _coursesTimetable.TryAdd(course.Id, timetable);
        _teachersTimetable.TryAdd(course.Teacher.Code, timetable);
        _studentsTimetable.TryAdd((course.DepartmentId, course.GradeYear), timetable);
        foreach (var room in timetable.Classrooms) {
            _classroomsTimetable.TryAdd(room.Id, timetable);
        }
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
    public Classroom? FindFreeRoomWithMatchedCapacity(Course course, LessonType lessonType, ScheduleTimeRange timeRange, Round round,
        Func<int, int, bool> capacityCheck)
    {
        var freeRooms = GetFreeRoomsByCourse(course, lessonType, timeRange, round);
        return freeRooms.FirstOrDefault(x => capacityCheck(course.MaxStudentsNumber, x.Capacity));
    }
    
    /// <summary>
    /// Finds room with capacity that is enough for the course 
    /// </summary>
    /// <param name="course"></param>
    /// <param name="lessonType"></param>
    /// <param name="timeRange"></param>
    /// <param name="round"></param>
    /// <returns></returns>
    public Classroom? FindFreeRoomWithMatchedCapacity(Course course, LessonType lessonType, ScheduleTimeRange timeRange, Round round)
    {
        var freeRooms = GetFreeRoomsByCourse(course, lessonType, timeRange, round);
        return freeRooms.FirstOrDefault(x => course.MaxStudentsNumber <= x.Capacity + 10);
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
    public List<Classroom>? FindTwoFreeRoomsWithMatchedCapacity(Course course, LessonType lessonType, ScheduleTimeRange timeRange, Round round,
        Func<int, int, bool> capacityCheck)
    {
        var freeRooms = GetFreeRoomsByCourse(course, lessonType, timeRange, round);
        // Todo: I didn't understand why we do so
        foreach (var byBuilding in freeRooms.GroupBy(x => x.BuildingId)) {
            var rooms = byBuilding.ToList();
            for (int col = 0; col < rooms.Count; col++) {
                for (int row = 0; row < col; row++) {
                    var (rowRoom, colRoom) = (rooms[row], rooms[col]);
                    int currentCapacity = rowRoom.Capacity + colRoom.Capacity;
                    if (capacityCheck(course.MaxStudentsNumber, currentCapacity)) 
                        return new List<Classroom> { rowRoom, colRoom };
                }
            }
        }
        return null;
    }
    
    /// <summary>
    /// Finds two rooms with total capacity that is enough for the course 
    /// </summary>
    /// <param name="course"></param>
    /// <param name="lessonType"></param>
    /// <param name="timeRange"></param>
    /// <param name="round"></param>
    /// <returns></returns>
    public List<Classroom>? FindTwoFreeRoomsWithMatchedCapacity(Course course, LessonType lessonType, ScheduleTimeRange timeRange, Round round)
    {
        var freeRooms = GetFreeRoomsByCourse(course, lessonType, timeRange, round);
        // Todo: I didn't understand why we do so
        foreach (var byBuilding in freeRooms.GroupBy(x => x.BuildingId)) {
            var rooms = byBuilding.ToList();
            for (int col = 0; col < rooms.Count; col++) {
                for (int row = 0; row < col; row++) {
                    var (rowRoom, colRoom) = (rooms[row], rooms[col]);
                    int currentCapacity = rowRoom.Capacity + colRoom.Capacity;
                    if (course.MaxStudentsNumber <= currentCapacity + 10) 
                        return new List<Classroom> { rowRoom, colRoom };
                }
            }
        }
        return null;
    }

    private IEnumerable<Classroom> GetFreeRoomsByCourse(Course course, LessonType lessonType, ScheduleTime time, Round round)
    {
        var roomsForSpecialCourses = course.GetRoomsForSpecialCourses(lessonType);
        var rooms = roomsForSpecialCourses.Any() ? roomsForSpecialCourses 
            : _classroomsData.GetClassrooms(course, lessonType, round);
        return rooms.Where(x => RoomIsFree(x, time));
    }
    
    private IEnumerable<Classroom> GetFreeRoomsByCourse(Course course, LessonType lessonType, ScheduleTimeRange timeRange, Round round)
    {
        var roomsForSpecialCourses = course.GetRoomsForSpecialCourses(lessonType);
        var rooms = roomsForSpecialCourses.Any() ? roomsForSpecialCourses 
            : _classroomsData.GetClassrooms(course, lessonType, round);
        return rooms.Where(x => RoomIsFree(x, timeRange));
    }

    /// <summary>
    /// Checks if course, students and teacher are free at the given time
    /// </summary>
    /// <param name="course"></param>
    /// <param name="timeRange"></param>
    /// <param name="hoursNeeded"></param>
    /// <param name="round"></param>
    /// <returns></returns>
    public bool ScheduleTimeRangeIsFree(Course course, ScheduleTimeRange timeRange, Round round)
    {
        foreach (var currentTime in timeRange.GetScheduleTimes()) {
            //check if the course free at that time (may be the uygulama lesson at the same time but in the other room)
            if (!CourseIsFree(course.Id, currentTime)) return false;

            // check if the students are free at the given time
            // Todo: keep <round> logic in one place
            if (!StudentsAreFree(course, currentTime, round)) return false;

            // check if the teacher is free at the given time
            if (!TeacherIsFree(course.Teacher, currentTime)) return false;
        }

        return true;
    }
    
    /// <summary>
    /// Checks if the course is free at that time (may be the practice lesson at the same time but in the other room)
    /// </summary>
    /// <param name="courseId"></param>
    /// <param name="time"></param>
    /// <returns>True if the course is free</returns>
    public bool CourseIsFree(int courseId, ScheduleTime time) => !_coursesTimetable[courseId].ContainsKey(time);
    
    /// <summary>
    /// Checks if the room is free at that time
    /// </summary>
    /// <param name="classroom"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public bool RoomIsFree(Classroom classroom, ScheduleTime time) => !_classroomsTimetable[classroom.Id].ContainsKey(time);
    
    /// <summary>
    /// Checks if the room is free at that time range
    /// </summary>
    /// <param name="classroom"></param>
    /// <param name="timeRange"></param>
    /// <returns></returns>
    public bool RoomIsFree(Classroom classroom, ScheduleTimeRange timeRange)
    {
        return timeRange.GetScheduleTimes().All(x => RoomIsFree(classroom, x));
    }

    /// <summary>
    /// Checks if students of the given year level and department are free at the given time
    /// </summary>
    /// <param name="course"></param>
    /// <param name="scheduleTime"></param>
    /// <param name="round"></param>
    /// <returns>True if students are free</returns>
    public bool StudentsAreFree(Course course, ScheduleTime scheduleTime, Round round)
    {
        var timetablesByDate = _studentsTimetable[(course.DepartmentId, course.GradeYear)][scheduleTime];

        if (course.CourseType == CourseType.DepartmentElective && round > Round.First)
            return timetablesByDate.TrueForAll(x => x.Course.CourseType == CourseType.DepartmentElective);
        
        return timetablesByDate.Count == 0;
    }
    
    /// <summary>
    /// Checks if the teacher is free at the given time.
    /// </summary>
    /// <param name="teacher"></param>
    /// <param name="time"></param>
    /// <returns>True if the teacher is free</returns>
    public bool TeacherIsFree(Teacher teacher, ScheduleTime time) => !_teachersTimetable[teacher.Code].ContainsKey(time);
    
    /// <summary>
    /// Returns remaining hours for course by lesson type
    /// </summary>
    /// <param name="course"></param>
    /// <param name="lessonType"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public int RemainingHoursByLessonType(Course course, LessonType lessonType) => lessonType switch {
        // If there is only 1 hour for theory lesson, may be it is better to put it together with practice lessons
        LessonType.Theory when UnpositionedTheoryHours(course) == 1 && course.PracticeHours is not 0 and < 3 &&
                               course.TheoryRoomType == course.PracticeRoomType => 0,
        LessonType.Practice when UnpositionedTheoryHours(course) == 1 && course.PracticeHours is not 0 and < 3 && 
                                 course.TheoryRoomType == course.PracticeRoomType => UnpositionedPracticeHours(course) + 1,
        
        LessonType.Theory => UnpositionedTheoryHours(course),
        LessonType.Practice => UnpositionedPracticeHours(course),
        _ => throw new ArgumentOutOfRangeException(nameof(lessonType), lessonType, "Not handled all lesson types!"),
    };

    private int UnpositionedTheoryHours(Course course) => 
        course.TheoryHours - _coursesTimetable[course.Id].Values.Count(x => x.LessonType == LessonType.Theory);
    private int UnpositionedPracticeHours(Course course) => 
        course.PracticeHours - _coursesTimetable[course.Id].Values.Count(x => x.LessonType == LessonType.Practice);

    private class TimetableByTimeByKey<TKey> : Dictionary<TKey, TimetableByTime> where TKey : notnull
    {
        public bool TryAdd(TKey key, Timetable timetable)
        {
            if (ContainsKey(key)) return this[key].TryAdd(timetable.ScheduleTime, timetable);
            this[key] = new TimetableByTime {
                [timetable.ScheduleTime] = timetable
            };
            return true;
        }
    }

    private class TimetablesByTimeByKey<TKey> : Dictionary<TKey, TimetablesByTime> where TKey : notnull
    {
        public bool TryAdd(TKey key, Timetable timetable)
        {
            if (ContainsKey(key)) {
                var timetablesByTime = this[key];
                if (timetablesByTime.ContainsKey(timetable.ScheduleTime))
                    timetablesByTime[timetable.ScheduleTime].Add(timetable);

                timetablesByTime[timetable.ScheduleTime] = new List<Timetable> {
                    timetable
                };
                return true;
            }

            this[key] = new TimetablesByTime {
                [timetable.ScheduleTime] = new() {
                    timetable
                }
            };
            return true;
        }
    }
    
    private class TimetableByTime : Dictionary<ScheduleTime, Timetable> { }
    private class TimetablesByTime : Dictionary<ScheduleTime, List<Timetable>> { }
}
