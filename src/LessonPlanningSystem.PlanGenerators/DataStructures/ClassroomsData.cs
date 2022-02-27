using LessonPlanningSystem.PlanGenerators.Enums;
using LessonPlanningSystem.PlanGenerators.Models;
using static LessonPlanningSystem.PlanGenerators.Configuration.StaticConfiguration;

namespace LessonPlanningSystem.PlanGenerators.DataStructures;

public class ClassroomsData
{
    private readonly Dictionary<int, Classroom> _allClassrooms;
    
    public IReadOnlyDictionary<int, Classroom> AllClassrooms => _allClassrooms;
    public IReadOnlyList<Classroom> SortedByCapacity { get; private set; }
    
    public readonly Action AddingEnded;

    public ClassroomsData()
    {
        _allClassrooms = new Dictionary<int, Classroom>();
        AddingEnded += SortByCapacity;
    }

    public bool Add(Classroom classroom)
    {
        return _allClassrooms.TryAdd(classroom.Id, classroom);
    }
    
    private void SortByCapacity()
    {
        SortedByCapacity = _allClassrooms.Values.OrderBy(x => x.Capacity).ToList();
    }

    /// <summary>
    /// Generate list of rooms for lessons of this course
    /// </summary>
    /// <param name="course"></param>
    /// <param name="lessonType"></param>
    /// <param name="round"></param>
    /// <returns></returns>
    /// Todo: make pre-calculation for each course. Maybe in multi-threading
    public IEnumerable<Classroom> GenerateRoomsList(Course course, LessonType lessonType, int round)
    {
        return from classroom in SortedByCapacity 
            let roomTypeMatch = RoomTypeCheck(course.NeededRoomType(lessonType), classroom.RoomType, round)
            let facultyDistance = course.Faculty.Building.Distance
            let classroomDistance = classroom.Building.Distance
            let departmentMatch = round switch {
                <= 3 => course.FacultyId == classroom.Department.FacultyId, //check if the course and the room are belong to the same faculty
                4 => classroom.BuildingId == course.Faculty.BuildingId, // In the fourth round we try to find rooms from the same building
                _ => Math.Abs(facultyDistance - classroomDistance) <= RadiusAroundBuilding, // If round 5 than we should find rooms from neighbour buildings also
            } where roomTypeMatch && departmentMatch select classroom;
    }
    
    /// <summary>
    /// This function checks the room type
    /// </summary>
    /// <param name="roomTypeNeeded"></param>
    /// <param name="currentRoomType"></param>
    /// <param name="round"></param>
    /// <returns></returns>
    private bool RoomTypeCheck(RoomType roomTypeNeeded, RoomType currentRoomType, int round) => round switch {
        // If room type needed is equal to 1, it meens "herhangi bir oda"
        <= 2 when roomTypeNeeded == RoomType.Normal => 
            currentRoomType is RoomType.Normal or RoomType.WithTwoBoards or RoomType.WithProjector,
        //check for the room type needed for the TEORIK lessons of the current course
        <= 2 => roomTypeNeeded == currentRoomType,
        3 => roomTypeNeeded switch {
            // This part is for round 3 (same faculty) with lite extend
            RoomType.Normal or RoomType.WithTwoBoards => currentRoomType is RoomType.Normal or RoomType.WithTwoBoards,
            RoomType.WithProjector or RoomType.WithSmartBoardAndProjector => currentRoomType is RoomType.WithProjector
                or RoomType.WithSmartBoardAndProjector,
            _ => roomTypeNeeded == currentRoomType,
        },
        _ => roomTypeNeeded switch {
            // This is for round 4 and 5 with full extend -> this meens that we are looking for room in other buildings also
            RoomType.Normal => currentRoomType is RoomType.Normal or RoomType.WithTwoBoards or RoomType.WithProjector,
            RoomType.WithTwoBoards => currentRoomType is RoomType.Normal or RoomType.WithTwoBoards,
            RoomType.WithProjector or RoomType.WithSmartBoardAndProjector => currentRoomType is RoomType.Normal
                or RoomType.WithTwoBoards or RoomType.WithProjector or RoomType.WithSmartBoardAndProjector,
            _ => roomTypeNeeded == currentRoomType,
        },
    };
}
