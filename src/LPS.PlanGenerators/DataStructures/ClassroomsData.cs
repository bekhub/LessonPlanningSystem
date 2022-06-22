using System.Collections.Concurrent;
using System.Collections.Immutable;
using LPS.PlanGenerators.Enums;
using LPS.PlanGenerators.Models;
using static LPS.PlanGenerators.Configuration.StaticConfiguration;

namespace LPS.PlanGenerators.DataStructures;

public class ClassroomsData
{
    private readonly Dictionary<int, Classroom> _allClassrooms;
    private IReadOnlyDictionary<(int, Round), IReadOnlyList<Classroom>> _theoryClassrooms;
    private IReadOnlyDictionary<(int, Round), IReadOnlyList<Classroom>> _practiceClassrooms;
    
    private ImmutableList<Classroom> _sortedByCapacity;

    public IReadOnlyDictionary<int, Classroom> AllClassrooms => _allClassrooms;
    public IReadOnlyList<Classroom> AllClassroomList => _allClassrooms.Values.ToList();

    public ClassroomsData()
    {
        _allClassrooms = new Dictionary<int, Classroom>();
    }

    public bool Add(Classroom classroom)
    {
        return _allClassrooms.TryAdd(classroom.Id, classroom);
    }
    
    public void AddingEnded(IReadOnlyList<Course> courses)
    {
        if (_allClassrooms.Count == 0) throw new InvalidOperationException("Classrooms must be added!");
        _sortedByCapacity = _allClassrooms.Values.OrderBy(x => x.Capacity).ToImmutableList();
        GenerateRoomListForCourses(courses);
    }
    
    public IReadOnlyList<Classroom> GetClassroomsByCourse(Course course, LessonType lessonType, Round round) =>
        lessonType switch {
            LessonType.Theory => round switch {
                <= Round.Third => _theoryClassrooms[(course.Id, Round.Third)],
                > Round.Third => _theoryClassrooms[(course.Id, round)],
            },
            LessonType.Practice => round switch {
                <= Round.Third => _practiceClassrooms[(course.Id, Round.Third)],
                > Round.Third => _practiceClassrooms[(course.Id, round)],
            },
            _ => throw new ArgumentOutOfRangeException(nameof(lessonType), lessonType, null),
        };

    private void GenerateRoomListForCourses(IReadOnlyList<Course> courses)
    {
        var concurrentTheoryClassrooms = new ConcurrentDictionary<(int, Round), IReadOnlyList<Classroom>>();
        var concurrentPracticeClassrooms = new ConcurrentDictionary<(int, Round), IReadOnlyList<Classroom>>();
        Parallel.ForEach(courses, course => {
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
        foreach (var classroom in _sortedByCapacity) {
            var roomTypeMatch = RoomTypeCheck(course.NeededRoomType(lessonType), classroom.RoomType, round);
            var facultyDistance = course.Department.Faculty.Building.Distance;
            var classroomDistance = classroom.Building.Distance;
            var departmentMatch = round switch {
                <= Round.Third => course.Department.Faculty.Id == classroom.Department.Faculty.Id, //check if the course and the room are belong to the same faculty
                Round.Fourth => classroom.Building.Id == course.Department.Faculty.Building.Id, // In the fourth round we try to find rooms from the same building
                _ => Math.Abs(facultyDistance - classroomDistance) <= RadiusAroundBuilding, // If round 5 than we should find rooms from neighbour buildings also
            };
            if (roomTypeMatch && departmentMatch) yield return classroom;
        }
    }
    
    /// <summary>
    /// This function checks the room type
    /// </summary>
    /// <param name="roomTypeNeeded"></param>
    /// <param name="currentRoomType"></param>
    /// <param name="round"></param>
    /// <returns></returns>
    private bool RoomTypeCheck(RoomType? roomTypeNeeded, RoomType currentRoomType, Round round) => round switch {
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
