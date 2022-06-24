#nullable enable
using LPS.PlanGenerators.Enums;
using LPS.PlanGenerators.Models;

namespace LPS.PlanGenerators.DataStructures.Extensions;

public static class ClassroomExtensions
{
    /// <summary>
    /// Finds one or two rooms with total capacity that is enough for the course 
    /// </summary>
    public static (Classroom Room, Classroom? Additional)? RoomsWithMatchedCapacity(this IReadOnlyList<Classroom> freeRooms, 
        Course course, LessonType lessonType, Func<int, int, bool>? capacityCheck = null)
    {
        var freeRoom = freeRooms.RoomWithMatchedCapacity(course, capacityCheck);
        if (freeRoom != null) return (freeRoom, null);
        if (lessonType == LessonType.Practice && course.PracticeRoomType is RoomType.WithComputers or RoomType.Laboratory)
            return freeRooms.TwoRoomsWithMatchedCapacity(course, capacityCheck);
        return null;
    }
    
    /// <summary>
    /// Finds one or two rooms with total capacity that is enough for each course 
    /// </summary>
    public static IReadOnlyList<(Course, (Classroom, Classroom?))>? RoomsWithMatchedCapacity(
        this IReadOnlyList<Classroom> freeRooms, IReadOnlyList<Course> mergedCourses, LessonType lessonType,
        Func<int, int, bool>? capacityCheck = null)
    {
        var coursesRooms = new List<(Course, (Classroom, Classroom?))>();
        var rooms = freeRooms.ToDictionary(x => x.Id);
        var courses = mergedCourses.ToDictionary(x => x.Id);
        foreach (var course in mergedCourses) {
            var freeRoom = rooms.Values.RoomWithMatchedCapacity(course, capacityCheck);
            if (freeRoom == null) continue;

            rooms.Remove(freeRoom.Id);
            courses.Remove(course.Id);
            coursesRooms.Add((course, (freeRoom, null)));
        }

        foreach (var course in courses.Values) {
            if (lessonType != LessonType.Practice || 
                course.PracticeRoomType is not (RoomType.WithComputers or RoomType.Laboratory)) return null;
            var matchedRooms = rooms.Values.TwoRoomsWithMatchedCapacity(course, capacityCheck);
            if (matchedRooms == null) return null;
            var (room, additional) = matchedRooms.Value;
            rooms.Remove(room.Id);
            rooms.Remove(additional.Id);
            coursesRooms.Add((course, matchedRooms.Value));
        }
        return coursesRooms;
    }
    
    /// <summary>
    /// Finds room with capacity that is enough for the course
    /// </summary>
    public static Classroom? RoomWithMatchedCapacity(this IEnumerable<Classroom> classrooms, Course course, 
        Func<int, int, bool>? capacityCheck = null)
    {
        return capacityCheck == null
            ? classrooms.FirstOrDefault(x => course.MaxStudentsNumber <= x.Capacity + 10)
            : classrooms.FirstOrDefault(x => capacityCheck(course.MaxStudentsNumber, x.Capacity));
    }
    
    /// <summary>
    /// Finds two rooms with total capacity that is enough for the course 
    /// </summary>
    public static (Classroom, Classroom)? TwoRoomsWithMatchedCapacity(this IEnumerable<Classroom> classrooms, 
        Course course, Func<int, int, bool>? capacityCheck = null)
    {
        foreach (var byBuilding in classrooms.GroupBy(x => x.Building.Id)) {
            var rooms = byBuilding.ToList();
            for (int col = 0; col < rooms.Count; col++) {
                for (int row = 0; row < col; row++) {
                    var (rowRoom, colRoom) = (rooms[row], rooms[col]);
                    int currentCapacity = rowRoom.Capacity + colRoom.Capacity;
                    if (capacityCheck == null && course.MaxStudentsNumber <= currentCapacity + 10)
                        return (rowRoom, colRoom);
                    if (capacityCheck != null && capacityCheck(course.MaxStudentsNumber, currentCapacity)) 
                        return (rowRoom, colRoom);
                }
            }
        }
        return null;
    }
}
