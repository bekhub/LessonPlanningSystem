using LessonPlanningSystem.PlanGenerators.ValueObjects;
using LessonPlanningSystem.PlanGenerators.DataStructures;
using LessonPlanningSystem.PlanGenerators.Configuration;
using LessonPlanningSystem.PlanGenerators.Enums;
using LessonPlanningSystem.PlanGenerators.Models;

namespace LessonPlanningSystem.PlanGenerators.Generators;

public class RandomPlanGenerator : IPlanGenerator
{
    private readonly CoursesData _coursesData;
    private readonly ClassroomsData _classroomsData;
    private readonly TimetableData _timetableData;
    private readonly BuildingsData _buildingsData;
    private readonly TeachersData _teachersData;
    private readonly PlanConfiguration _configuration;
    
    public RandomPlanGenerator(CoursesData coursesData, PlanConfiguration configuration, ClassroomsData classroomsData, 
        BuildingsData buildingsData, TeachersData teachersData, TimetableData timetableData)
    {
        _coursesData = coursesData;
        _configuration = configuration;
        _classroomsData = classroomsData;
        _buildingsData = buildingsData;
        _teachersData = teachersData;
        _timetableData = timetableData;
    }

    public LessonPlan GenerateBestLessonPlan()
    {
        var options = new ParallelOptions {
            MaxDegreeOfParallelism = _configuration.MaxNumberOfThreads ?? Environment.ProcessorCount - 1,
        };
        Parallel.For(0, _configuration.NumberOfVariants, options, _ => {
            
        });
        throw new NotImplementedException();
    }

    private void GenerateLessonPlan()
    {
        
    }

    private void FindPlaceForLesson(Course course, LessonType lessonType, Round round)
    {
        var currSgMode = course.SubgroupMode;
        if (currSgMode == SubgroupMode.Mode1 && lessonType == LessonType.Practice) {
            var hoursNeeded = _timetableData.RemainingHoursByLessonType(course, lessonType);
            if (hoursNeeded <= 0 || 
                // The following case needed in case if there is only 1 teorik hour and it has been added to uygulama hours in the method RemainingHoursByLessonType
                hoursNeeded % course.PracticeHours != 0) return;
            foreach (var timeRange in ScheduleTimeRange.GetWeekScheduleTimeRanges(hoursNeeded)) {
                if (!_timetableData.ScheduleTimeRangeIsFree(course, timeRange, round) ||
                    // Todo: why we do not use following?!
                    // !course.TimeRangeIsConvenientForCourse(timeRange, round) ||
                    !course.TimeIsConvenientForCourse(timeRange.GetScheduleTimes().First(), round)) continue;

                var rooms = FindFreeRoomsWithMatchedCapacity(course, lessonType, timeRange, round, 
                    (courseStudentsNumber, roomCapacity) => courseStudentsNumber / 2 <= roomCapacity + 10);
                if (rooms == null || rooms.Count == 0) continue;
                foreach (var time in timeRange.GetScheduleTimes()) {
                    _timetableData.AddTimetable(new Timetable {
                        Course = course,
                        Classrooms = rooms,
                        LessonType = lessonType,
                        ScheduleTime = time,
                    });
                }
                if (_timetableData.RemainingHoursByLessonType(course, lessonType) <= 0) break;
            }
        }
        else if (currSgMode <= SubgroupMode.Mode2 || currSgMode == SubgroupMode.Mode6 && lessonType == LessonType.Practice) {
            var hoursNeeded = _timetableData.RemainingHoursByLessonType(course, lessonType);
            if (hoursNeeded <= 0) return;
            foreach (var timeRange in ScheduleTimeRange.GetWeekScheduleTimeRanges(hoursNeeded)) {
                if (!_timetableData.ScheduleTimeRangeIsFree(course, timeRange, round) ||
                    // !course.TimeRangeIsConvenientForCourse(timeRange, round) ||
                    !course.TimeIsConvenientForCourse(timeRange.GetScheduleTimes().First(), round)) continue;
                
                var rooms = FindFreeRoomsWithMatchedCapacity(course, lessonType, timeRange, round);
                if (rooms == null || rooms.Count == 0) continue;
                foreach (var time in timeRange.GetScheduleTimes()) {
                    _timetableData.AddTimetable(new Timetable {
                        Course = course,
                        Classrooms = rooms,
                        LessonType = lessonType,
                        ScheduleTime = time,
                    });
                }
                if (_timetableData.RemainingHoursByLessonType(course, lessonType) <= 0) break;
            }
        }
    }

    private List<Classroom> FindFreeRoomsWithMatchedCapacity(Course course, LessonType lessonType, ScheduleTimeRange timeRange, Round round,
        Func<int, int, bool> capacityCheck)
    {
        var freeRoom = _timetableData.FindFreeRoomWithMatchedCapacity(course, lessonType, timeRange, round, capacityCheck);
        if (freeRoom != null) return new List<Classroom> { freeRoom };
        if (lessonType == LessonType.Practice && course.PracticeRoomType is RoomType.WithComputers or RoomType.Laboratory)
            return _timetableData.FindTwoFreeRoomsWithMatchedCapacity(course, lessonType, timeRange, round, capacityCheck);
        return null;
    }
    
    private List<Classroom> FindFreeRoomsWithMatchedCapacity(Course course, LessonType lessonType, ScheduleTimeRange timeRange, Round round)
    {
        var freeRoom = _timetableData.FindFreeRoomWithMatchedCapacity(course, lessonType, timeRange, round);
        if (freeRoom != null) return new List<Classroom> { freeRoom };
        if (lessonType == LessonType.Practice && course.PracticeRoomType is RoomType.WithComputers or RoomType.Laboratory)
            return _timetableData.FindTwoFreeRoomsWithMatchedCapacity(course, lessonType, timeRange, round);
        return null;
    }
}
