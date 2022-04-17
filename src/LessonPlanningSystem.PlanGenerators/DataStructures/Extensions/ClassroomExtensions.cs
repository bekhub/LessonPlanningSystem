#nullable enable
using LessonPlanningSystem.PlanGenerators.Models;

namespace LessonPlanningSystem.PlanGenerators.DataStructures.Extensions;

public static class ClassroomExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="classrooms"></param>
    /// <param name="course"></param>
    /// <param name="capacityCheck"></param>
    /// <returns></returns>
    public static Classroom? RoomWithMatchedCapacity(this IEnumerable<Classroom> classrooms, Course course, 
        Func<int, int, bool>? capacityCheck = null)
    {
        return capacityCheck == null
            ? classrooms.FirstOrDefault(x => course.MaxStudentsNumber <= x.Capacity + 10)
            : classrooms.FirstOrDefault(x => capacityCheck(course.MaxStudentsNumber, x.Capacity));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="classrooms"></param>
    /// <param name="course"></param>
    /// <param name="capacityCheck"></param>
    /// <returns>Two </returns>
    public static IReadOnlyList<Classroom>? RoomsWithMatchedCapacity(this IEnumerable<Classroom> classrooms, 
        Course course, Func<int, int, bool>? capacityCheck = null)
    {
        foreach (var byBuilding in classrooms.GroupBy(x => x.Building.Id)) {
            var rooms = byBuilding.ToList();
            for (int col = 0; col < rooms.Count; col++) {
                for (int row = 0; row < col; row++) {
                    var (rowRoom, colRoom) = (rooms[row], rooms[col]);
                    int currentCapacity = rowRoom.Capacity + colRoom.Capacity;
                    if (capacityCheck == null && course.MaxStudentsNumber <= currentCapacity + 10)
                        return new List<Classroom> {rowRoom, colRoom};
                    if (capacityCheck != null && capacityCheck(course.MaxStudentsNumber, currentCapacity)) 
                        return new List<Classroom> {rowRoom, colRoom};
                }
            }
        }
        return null;
    }
}
