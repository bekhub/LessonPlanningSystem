using LessonPlanningSystem.PlanGenerators.Configuration;
using LessonPlanningSystem.PlanGenerators.Enums;
using LessonPlanningSystem.PlanGenerators.Models;

namespace LessonPlanningSystem.PlanGenerators.DataStructures;

public class ClassroomsData
{
    private readonly Dictionary<int, Classroom> _allClassrooms;
    private readonly PlanConfiguration _configuration;

    public IReadOnlyDictionary<int, Classroom> AllClassrooms => _allClassrooms;

    public ClassroomsData(PlanConfiguration configuration)
    {
        _configuration = configuration;
        _allClassrooms = new Dictionary<int, Classroom>();
    }

    public bool Add(Classroom classroom)
    {
        return _allClassrooms.TryAdd(classroom.Id, classroom);
    }
    
    // This function generates the list of the rooms to use for the given course. if lessonType=0 -> teorik
    public List<int> GenerateRoomsList(Course course, LessonType lessonType, int round) {
        var roomIdListSortedByCapacity = new List<(int roomId, int capacity)>(AllClassrooms.Count);

        foreach (var classroom in AllClassrooms.Values) {
            var currentRoomType = classroom.RoomType;
            
            var roomTypeNeeded = lessonType == LessonType.Theory ? course.TheoryRoomType : course.PracticeRoomType;

            var roomTypeMatch = RoomTypeCheck(roomTypeNeeded, currentRoomType, round); //, roomCapacityMatch;

            bool departmentMatch;
            switch (round) {
                case <= 3:
                    departmentMatch = course.FacultyId == classroom.Department.FacultyId;        //check if the course and the room are belong to the same faculty
                    break;
                case 4: // In the fourth round we try to find rooms from the same building
                    departmentMatch = classroom.BuildingId == course.Faculty.BuildingId;
                    break;
                default: {
                    // If round 5 than we should find rooms from neighbour buildings also
                    var currentRoomDistanceInfo = classroom.Building.DistanceNumber;
                    var facultyDistanceInfo = course.Faculty.Building.DistanceNumber;
                    departmentMatch = Math.Abs(facultyDistanceInfo - currentRoomDistanceInfo) <= _configuration.RadiusAroundBuilding;
                    break;
                }
            }

            if (roomTypeMatch && departmentMatch) {
                roomIdListSortedByCapacity.Add((classroom.Id, classroom.Capacity));
            }
        }

        return roomIdListSortedByCapacity.Count == 0 ? null : roomIdListSortedByCapacity
            .OrderBy(x => x.capacity)
            .Select(x => x.roomId)
            .ToList();
    }
    
    // This function checks the room type
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
