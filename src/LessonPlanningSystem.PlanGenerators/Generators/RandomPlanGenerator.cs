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
        if (currSgMode <= SubgroupMode.Mode2 || currSgMode == SubgroupMode.Mode6 && lessonType == LessonType.Practice) {
            var hoursNeeded = _timetableData.RemainingHoursByLessonType(course, lessonType);
            if (hoursNeeded <= 0) return;
            foreach (var time in ScheduleTime.GetWeekScheduleTimes()) {
                if (!ScheduleTime.NotLunchOrEndOfDay(time.Hour, hoursNeeded) || 
                    !_timetableData.ScheduleTimeIsFree(course, time, hoursNeeded, round) ||
                    //if(!this.checkHourIsConvenientForCourse(i, hour+hoursNeeded, round)) hourIsConvenientForCourse = false;
                    !course.TimeIsConvenientForCourse(time, round)) continue;
                var rooms = FindFreeRooms(course, lessonType, time, round);
                if (rooms == null || rooms.Count == 0) continue;
                _timetableData.AddTimetable(new Timetable {
                    Course = course,
                    Classrooms = rooms,
                    LessonType = lessonType,
                    ScheduleTime = time,
                });
                break;
            }
        }
    }

    private List<Classroom> FindFreeRooms(Course course, LessonType lessonType, ScheduleTime time, Round round)
    {
        var freeRoom = _timetableData.FindFreeRoomWithMatchedCapacity(course, lessonType, time, round);
        if (freeRoom != null) return new List<Classroom> { freeRoom };
        if (lessonType == LessonType.Practice && course.PracticeRoomType is RoomType.WithComputers or RoomType.Laboratory)
            return _timetableData.FindTwoFreeRoomsWithMatchedCapacity(course, lessonType, time, round);
        return null;
    }
}
