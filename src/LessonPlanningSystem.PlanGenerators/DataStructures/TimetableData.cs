using System.Diagnostics.CodeAnalysis;
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
    public Dictionary<int, Dictionary<ScheduleTime, Timetable>> CoursesTimetable = new();
    /// <summary>
    /// Timetables by classroom id. Classroom can be used only once at the same time
    /// </summary>
    public Dictionary<int, Dictionary<ScheduleTime, Timetable>> ClassroomsTimetable = new();
    /// <summary>
    /// Timetables by teacher code. Teacher can be in only one room at the same time
    /// </summary>
    public Dictionary<int, Dictionary<ScheduleTime, Timetable>> TeachersTimetable = new();
    /// <summary>
    /// Timetables by students(department id and grade year). Students can be in multiple rooms at the same time
    /// </summary>
    public Dictionary<(int DepartmentId, GradeYear GradeYear), List<Timetable>> StudentsTimetable = new();

    public TimetableData(ClassroomsData classroomsData) {
        _classroomsData = classroomsData;
    }

    public void AddTimetable(Timetable timetable)
    {
        throw new NotImplementedException();
    }
    
    // Todo: make optimization
    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    public List<Classroom> GenerateFreeRoomListForCourse(Course course, LessonType lessonType, ScheduleTime time, int round)
    {
        // Todo: multiple same generation for course. Change this implementation 
        var roomsForSpecialCourses = course.GetRoomsForSpecialCourses(lessonType).ToList();
        var rooms = roomsForSpecialCourses.Any() ? roomsForSpecialCourses 
            : _classroomsData.GenerateRoomsList(course, lessonType, round);
        var freeRooms = rooms.Where(x => !ClassroomsTimetable[x.Id].ContainsKey(time));
        var matchedByCapacity = freeRooms.Where(x => course.MaxStudentsNumber <= x.Capacity + 10).Take(1).ToList();
        if (lessonType == LessonType.Theory || matchedByCapacity.Count > 0 ||
            course.PracticeRoomType is not (RoomType.WithComputers or RoomType.Laboratory)) return matchedByCapacity;
        return TwoRoomsByCapacity(course, freeRooms);
    }

    //This function finds two rooms with total capacity that is enough for the course
    private List<Classroom> TwoRoomsByCapacity(Course course, IEnumerable<Classroom> freeRooms)
    {
        foreach (var byBuilding in freeRooms.GroupBy(x => x.BuildingId)) {
            var rooms = byBuilding.ToList();
            for (int col = 0; col < rooms.Count; col++) {
                for (int row = 0; row < col; row++) {
                    var (rowRoom, colRoom) = (rooms[row], rooms[col]);
                    int currentCapacity = rowRoom.Capacity + colRoom.Capacity;
                    if (course.MaxStudentsNumber <= currentCapacity + 10) return new() { rowRoom, colRoom };
                }
            }
        }
        return new List<Classroom>();
    }

    /// <summary>
    /// Checks if course, students and teacher are free at the given time
    /// </summary>
    /// <param name="course"></param>
    /// <param name="time"></param>
    /// <param name="hoursNeeded"></param>
    /// <param name="round"></param>
    /// <returns></returns>
    public bool ScheduleTimeIsFree(Course course, ScheduleTime time, int hoursNeeded, int round)
    {
        for (int l = 0; l < hoursNeeded; l++) {
            var currentTime = new ScheduleTime(time.Weekday, time.Hour + l);
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
    public bool CourseIsFree(int courseId, ScheduleTime time)
    {
        return !CoursesTimetable[courseId].ContainsKey(time);
    }

    /// <summary>
    /// Checks if students of the given year level and department are free at the given time
    /// </summary>
    /// <param name="course"></param>
    /// <param name="scheduleTime"></param>
    /// <param name="round"></param>
    /// <returns>True if students are free</returns>
    public bool StudentsAreFree(Course course, ScheduleTime scheduleTime, int round)
    {
        var timetablesByDate = StudentsTimetable[(course.DepartmentId, course.GradeYear)]
            .Where(x => x.ScheduleTime == scheduleTime).ToList();
        
        if (course.CourseType == CourseType.DepartmentElective && round > 1)
            return timetablesByDate.TrueForAll(x => x.Course.CourseType == CourseType.DepartmentElective);
        
        return timetablesByDate.Count == 0;
    }
    
    /// <summary>
    /// Checks if the teacher is free at the given time.
    /// </summary>
    /// <param name="teacher"></param>
    /// <param name="time"></param>
    /// <returns>True if the teacher is free</returns>
    public bool TeacherIsFree(Teacher teacher, ScheduleTime time) => !TeachersTimetable[teacher.Code].ContainsKey(time);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="course"></param>
    /// <param name="lessonType"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public int RemainingHoursByLessonType(Course course, LessonType lessonType) => lessonType switch {
        // If there is only 1 hour for teorik lesson, may be it is better to put it together with uygulama lessons
        LessonType.Theory when UnpositionedTheoryHours(course) == 1 && course.PracticeHours is not 0 and < 3 &&
                               course.TheoryRoomType == course.PracticeRoomType => 0,
        LessonType.Practice when UnpositionedTheoryHours(course) == 1 && course.PracticeHours is not 0 and < 3 && 
                                 course.TheoryRoomType == course.PracticeRoomType => UnpositionedPracticeHours(course) + 1,
        
        LessonType.Theory => UnpositionedTheoryHours(course),
        LessonType.Practice => UnpositionedPracticeHours(course),
        _ => throw new ArgumentOutOfRangeException(nameof(lessonType), lessonType, "Not handled all lesson types!"),
    };

    private int UnpositionedTheoryHours(Course course) => 
        course.TheoryHours - CoursesTimetable[course.Id].Values.Count(x => x.LessonType == LessonType.Theory);
    private int UnpositionedPracticeHours(Course course) => 
        course.PracticeHours - CoursesTimetable[course.Id].Values.Count(x => x.LessonType == LessonType.Practice);
}
