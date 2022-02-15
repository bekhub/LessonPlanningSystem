using LessonPlanningSystem.PlanGenerators.Configuration;
using LessonPlanningSystem.PlanGenerators.Enums;
using LessonPlanningSystem.PlanGenerators.Models;

namespace LessonPlanningSystem.PlanGenerators.DataStructures;

public class ClassroomsData
{
    private readonly Dictionary<int, Classroom> _allClassrooms;
    private readonly BuildingsData _buildingsData;
    private readonly PlanConfiguration _configuration;

    public IReadOnlyDictionary<int, Classroom> AllClassrooms => _allClassrooms;

    public ClassroomsData(BuildingsData buildingsData, PlanConfiguration configuration)
    {
        _buildingsData = buildingsData;
        _configuration = configuration;
        _allClassrooms = new Dictionary<int, Classroom>();
    }

    public bool Add(Classroom classroom)
    {
        return _allClassrooms.TryAdd(classroom.Id, classroom);
    }
    
    // This function generates the list of the rooms to use for the given course. if lessonType=0 -> teorik
    public List<int> GenerateRoomsList(Course course, LessonType lessonType, int round) {
        List<int> roomsList = new();
        List<int> capacityDifference = new();

        //If it is the fourth or more round we should find building ID of the Faculty to which current course belongs to.
        int facultyBuildingId = 0;
        if (round >= 4) {
            facultyBuildingId = course.Faculty.BuildingId;
        }

        bool roomTypeMatch, departmentMatch; //, roomCapasityMatch;
        foreach (var (j, classroom) in AllClassrooms) {
            var currentRoomType = classroom.RoomType;
            
            var roomTypeNeeded = lessonType == LessonType.Theory ? course.TheoryRoomType : course.PracticeRoomType;

            roomTypeMatch = RoomTypeCheck(roomTypeNeeded, currentRoomType, round);

            int currentRoomsBuildingId = classroom.BuildingId;
            if (round <= 3)
                departmentMatch = course.FacultyId == classroom.Department.FacultyId;        //check if the course and the room are belong to the same faculty
            else {
                if (round == 4) {    // In the fourth round we try to find rooms from the same building

                    departmentMatch = currentRoomsBuildingId == facultyBuildingId;
                } else {   // If round 5 than we should find rooms from neighbour buildings also
                    int currentRoomDistanceInfo = 0;
                    int facultyDistanceInfo = 0;
                    foreach (var building in _buildingsData.AllBuildings.Values) {
                        if (building.Id == currentRoomsBuildingId) {
                            currentRoomDistanceInfo = building.DistanceNumber;
                        }
                        if (building.Id == facultyBuildingId) {
                            facultyDistanceInfo = building.DistanceNumber;
                        }
                    }
                    departmentMatch = Math.Abs(facultyDistanceInfo - currentRoomDistanceInfo) <= _configuration.RadiusAroundBuilding;
                }
            }

            if (roomTypeMatch && departmentMatch) {
                roomsList.Add(j);                // Adding room index to the rooms list
                capacityDifference.Add(classroom.Capacity);    // - course.getMaxOgrenciSayisi());
            }
        }

        //If No convenient rooms have been found
        if (roomsList.Count == 0) {
            return null;
        }

        //Sort the list by Capasity Difference - ASC - buble algorithm
        for (int i = roomsList.Count; i >= 2; i--)
            for (int j = 0; j < i - 1; j++) {
                if (capacityDifference[j] > capacityDifference[j + 1]) {
                    int temp = capacityDifference[j];
                    capacityDifference[j] = capacityDifference[j + 1];
                    capacityDifference[j + 1] = temp;

                    temp = roomsList[j];
                    roomsList[j] = roomsList[j + 1];
                    roomsList[j+ 1] = temp;
                }
            }

        return roomsList;
    }
    
    // This function checks the room type
    bool RoomTypeCheck(RoomType roomTypeNeeded, RoomType currentRoomType, int round) => round switch {
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
