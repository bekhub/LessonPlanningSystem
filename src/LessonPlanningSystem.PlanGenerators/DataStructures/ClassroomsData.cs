using System.Collections.Concurrent;
using System.Collections.Immutable;
using LessonPlanningSystem.PlanGenerators.Configuration;
using LessonPlanningSystem.PlanGenerators.Enums;
using LessonPlanningSystem.PlanGenerators.Models;
using static LessonPlanningSystem.PlanGenerators.Configuration.StaticConfiguration;

namespace LessonPlanningSystem.PlanGenerators.DataStructures;

public class ClassroomsData
{
    private readonly Dictionary<int, Classroom> _allClassrooms;
    private IReadOnlyDictionary<(int, Round), IReadOnlyList<Classroom>> _theoryClassrooms;
    private IReadOnlyDictionary<(int, Round), IReadOnlyList<Classroom>> _practiceClassrooms;
    
    private ImmutableList<Classroom> _sortedByCapacity;

    public IReadOnlyDictionary<int, Classroom> AllClassrooms => _allClassrooms;

    private readonly CoursesData _coursesData;
    private readonly PlanConfiguration _configuration;

    public ClassroomsData(CoursesData coursesData, PlanConfiguration configuration)
    {
        _coursesData = coursesData;
        _configuration = configuration;
        _allClassrooms = new Dictionary<int, Classroom>();
    }

    public bool Add(Classroom classroom)
    {
        return _allClassrooms.TryAdd(classroom.Id, classroom);
    }
    
    public void AddingEnded()
    {
        if (_allClassrooms.Count == 0) throw new InvalidOperationException("Classrooms must be added!");
        _sortedByCapacity = _allClassrooms.Values.OrderBy(x => x.Capacity).ToImmutableList();
        GenerateRoomListForCourses();
    }
    
    public IReadOnlyList<Classroom> GetClassroomsByCourse(Course course, LessonType lessonType, Round round) =>
        lessonType switch {
            LessonType.Theory => _theoryClassrooms[(course.Id, round)],
            LessonType.Practice => _practiceClassrooms[(course.Id, round)],
            _ => throw new ArgumentOutOfRangeException(nameof(lessonType), lessonType, null),
        };

    private void GenerateRoomListForCourses()
    {
        var options = new ParallelOptions {
            MaxDegreeOfParallelism = _configuration.MaxNumberOfThreads ?? Environment.ProcessorCount - 1,
        };
        var concurrentTheoryClassrooms = new ConcurrentDictionary<(int, Round), IReadOnlyList<Classroom>>();
        var concurrentPracticeClassrooms = new ConcurrentDictionary<(int, Round), IReadOnlyList<Classroom>>();
        Parallel.ForEach(_coursesData.AllCourses.Values, options, course => {
            for (var round = Round.Third; round <= Round.Fifth; round++) {
                concurrentTheoryClassrooms[(course.Id, round)] = GenerateRooms(course, LessonType.Theory, round).ToList();
                concurrentPracticeClassrooms[(course.Id, round)] = GenerateRooms(course, LessonType.Practice, round).ToList();
            }
        });
        _theoryClassrooms = concurrentTheoryClassrooms;
        _practiceClassrooms = concurrentPracticeClassrooms;
    }

    /// <summary>
    /// Generate list of rooms for lessons of this course
    /// </summary>
    /// <param name="course"></param>
    /// <param name="lessonType"></param>
    /// <param name="round"></param>
    /// <returns></returns>
    private IEnumerable<Classroom> GenerateRooms(Course course, LessonType lessonType, Round round)
    {
        return from classroom in _sortedByCapacity 
            let roomTypeMatch = RoomTypeCheck(course.NeededRoomType(lessonType), classroom.RoomType, round)
            let facultyDistance = course.Faculty.Building.Distance
            let classroomDistance = classroom.Building.Distance
            let departmentMatch = round switch {
                <= Round.Third => course.FacultyId == classroom.Department.FacultyId, //check if the course and the room are belong to the same faculty
                Round.Fourth => classroom.BuildingId == course.Faculty.BuildingId, // In the fourth round we try to find rooms from the same building
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
    private bool RoomTypeCheck(RoomType roomTypeNeeded, RoomType currentRoomType, Round round) => round switch {
        // If room type needed is equal to 1, it meens "herhangi bir oda"
        <= Round.Second when roomTypeNeeded == RoomType.Normal => 
            currentRoomType is RoomType.Normal or RoomType.WithTwoBoards or RoomType.WithProjector,
        //check for the room type needed for the TEORIK lessons of the current course
        <= Round.Second => roomTypeNeeded == currentRoomType,
        Round.Third => roomTypeNeeded switch {
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
