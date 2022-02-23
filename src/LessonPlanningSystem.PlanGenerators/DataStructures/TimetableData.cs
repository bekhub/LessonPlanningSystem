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
    /// Timetables by classroom id
    /// </summary>
    public Dictionary<int, Dictionary<ScheduleTime, Timetable>> ClassroomsTimetable = new();
    /// <summary>
    /// Timetables by teacher code
    /// </summary>
    public Dictionary<int, Dictionary<ScheduleTime, Timetable>> TeachersTimetable = new();
    /// <summary>
    /// Timetables by students(department id and grade year)
    /// </summary>
    public Dictionary<(int DepartmentId, GradeYear GradeYear), List<Timetable>> StudentsTimetable = new();

    public TimetableData(ClassroomsData classroomsData) {
        _classroomsData = classroomsData;
    }
    
    public void GenerateFreeRoomListByCourse(Course course, LessonType lessonType, ScheduleTime time, int round)
    {
        var roomIds = course.GetRoomIdsForSpecialCourses(lessonType);
        if (!roomIds.Any()) roomIds = _classroomsData.GenerateRoomsList(course, lessonType, round);
        roomIds = roomIds.Where(id => !ClassroomsTimetable[id].ContainsKey(time)).ToList();
    }

    //This function checks capacity match for the given list of rooms and returns the list of convenient rooms
    private List<int> CheckCapacityMatch(Course course, List<int> listOfFreeRooms, LessonType lessonType) {
        var listOfRooms = new List<int>();

        if (lessonType == 0) {    // If the lesson type is Teorik lesson, then just simple check for capacity of room and course students number
            foreach (int j in listOfFreeRooms) {
                if (course.MaxStudentsNumber <= _classroomsData.AllClassrooms[j].Capacity + 10) {
                    listOfRooms.Add(j);
                    break;
                }
            }
        } else {    // This means if lesson type is Uygulama, then find suitable room, if no room found - try to find two rooms

            // at first let's try single room capacity match
            foreach (int j in listOfFreeRooms) {
                if (course.MaxStudentsNumber <= _classroomsData.AllClassrooms[j].Capacity + 10) {
                    listOfRooms.Add(j);
                    break;
                }
            }

            if (course.PracticeRoomType is RoomType.WithComputers or RoomType.Laboratory) {
                if (listOfRooms.Count == 0) {    // This means that there is not any room with enough capacity. So we need to find two rooms with total capacity that is enough for the course
                    int arrayDimension = listOfFreeRooms.Count;
                    var capacityTotals = new int[arrayDimension, arrayDimension];

                    for (int column = 0; column < arrayDimension; column++) {
                        for (int row = 0; row < arrayDimension; row++) {
                            if (row > column) break;
                            if (row == column)
                                capacityTotals[row, column] = _classroomsData.AllClassrooms[listOfFreeRooms[row]].Capacity;
                            else {
                                capacityTotals[row, column] = _classroomsData.AllClassrooms[listOfFreeRooms[row]].Capacity + _classroomsData.AllClassrooms[listOfFreeRooms[column]].Capacity;
                            }
                        }
                    }

                    bool roomsFound = false;
                    for (int column = 0; column < arrayDimension; column++) {
                        for (int row = 0; row < arrayDimension; row++) {
                            if (row > column) break;
                            if (course.MaxStudentsNumber <= capacityTotals[row, column] + 10) {
                                if (_classroomsData.AllClassrooms[listOfFreeRooms[row]].BuildingId == _classroomsData.AllClassrooms[listOfFreeRooms[column]].BuildingId) {
                                    listOfRooms.Add(listOfFreeRooms[row]);
                                    listOfRooms.Add(listOfFreeRooms[column]);
                                    roomsFound = true;
                                    break;
                                }
                            }
                        }
                        if (roomsFound) break;
                    }
                }

            }
        }
        return listOfRooms;
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
    public bool TeacherIsFree(Teacher teacher, ScheduleTime time) {
        return !TeachersTimetable[teacher.Code].ContainsKey(time);
    }
    
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
