using LessonPlanningSystem.PlanGenerators.DataStructures;
using LessonPlanningSystem.PlanGenerators.Configuration;
using LessonPlanningSystem.PlanGenerators.Enums;
using LessonPlanningSystem.PlanGenerators.Models;

namespace LessonPlanningSystem.PlanGenerators.Generators;

public class RandomPlanGenerator : IPlanGenerator
{
    private readonly CoursesData _coursesData;
    private readonly ClassroomsData _classroomsData;
    private readonly BuildingsData _buildingsData;
    private readonly PlanConfiguration _configuration;
    
    public RandomPlanGenerator(CoursesData coursesData, PlanConfiguration configuration, ClassroomsData classroomsData, BuildingsData buildingsData)
    {
        _coursesData = coursesData;
        _configuration = configuration;
        _classroomsData = classroomsData;
        _buildingsData = buildingsData;
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

    private void FindPlaceForLesson(LessonPlan lessonPlan, LessonType lessonType, int round)
    {
        var currSgMode = lessonPlan.Course.SubgroupMode;
        if (currSgMode <= SubgroupMode.Mode2 || currSgMode == SubgroupMode.Mode6 && lessonType == LessonType.Practice) {
            var hoursNeeded = lessonPlan.RemainingHoursByLessonType(lessonType);

            if (hoursNeeded > 0) {
                // checks if there are still unpositioned LESSONS
                // Here specialRequirementsForCourse(i, 0) generates list of special rooms for this type of lessons of this course
                var specialRoomsList = new List<int>();

                // specialRoomsList is the list of rooms to use.
                var roomList = lessonPlan.Course.GetRoomIdsForSpecialCourses(lessonType);

                if (roomList.Count == 0) {                                        //If there are NOT any special requirements for the given course and this type of lessons
                    specialRoomsList = this.GenerateRoomsList(lessonPlan.Course, lessonType, round);                // Generate list of rooms for lessons of this course
                    //this.courses[i].setComments("Not ozel! ");
                }

                if (specialRoomsList != null) {

                    for (int hour = 0; hour <= _configuration.WeekHoursNumber - hoursNeeded; hour++) {
                        var capacityMatch = false;
                        var courseIsFree = true;
                        var studentsAreFree = true;
                        var teacherIsFree = true;
                        var hourIsConvenientForCourse = true;

                        if (this.CheckTimeForLunchOrEndOfDay(hour, hoursNeeded)) {    //Here we chek if there is enough time until lunch or the end of the day for the hours needed
                            for (int l = 0; l < hoursNeeded; l++) {
                                //System.out.println("ders kodu: "+this.courses[i].getDersKodu()+" hoursNeeded: "+hoursNeeded+" hour: "+hour+" l=: "+l);
                                if (lessonPlan.getRoomID(hour + l) != 0)
                                    courseIsFree = false;    //check if the course free at that time (may be the uygulama lesson at the same time but in the other room)

                                // check if the students are free at the given time
                                if (!this.checkStudentsAreFree(courseIdx, lessonPlan.getBolumID(), getYearLevel(lessonPlan.getDersKodu()), hour + l, round))
                                    studentsAreFree = false;

                                // check if the teacher is free at the given time
                                if (!this.checkTeacherIsFree(lessonPlan.getHocaninSicilNumarasi(), hour + l))
                                    teacherIsFree = false;
                            }

                            // This function ensures that BZD of 1'st and 3'd level are positioned before noon, and BZD of 2'nd and 4'th level are positioned after noon
                            if (!this.checkHourIsConvenientForCourse(courseIdx, hour, round))
                                hourIsConvenientForCourse = false;
                            //if(!this.checkHourIsConvenientForCourse(i, hour+hoursNeeded, round)) hourIsConvenientForCourse = false;


                            // Let's create a list of free rooms at the given hour
                            ArrayList<Integer> listOfFreeRooms = new ArrayList<>();
                            //j => array index of the room
                            for (int j : specialRoomsList) {
                                var roomIsFree = true;
                                for (int l = 0; l < hoursNeeded; l++) {
                                    try {
                                        if (this.rooms[j].getRoomTime(hour + l) != 0)
                                            roomIsFree = false;        //check if the room is free at that time
                                    } catch (ArrayIndexOutOfBoundsException e) {
                                        System.out.println("Array Index out of bounds: Mode 1, Uygulama: j=" + j + " hour=" + hour + " course index=" + courseIdx);
                                    }
                                }
                                if (roomIsFree) listOfFreeRooms.add(j);
                            }

                            ArrayList<Integer> listOfCapacityMatchedRooms = new ArrayList<>();
                            if (listOfFreeRooms.size() != 0) {
                                listOfCapacityMatchedRooms = this.checkCapacityMatch(courseIdx, listOfFreeRooms, lessonType);
                                if (listOfCapacityMatchedRooms.size() > 0) capacityMatch = true;
                            }

                            if (capacityMatch && courseIsFree && studentsAreFree && teacherIsFree && hourIsConvenientForCourse) {
                                for (int l = 0; l < hoursNeeded; l++) {
                                    //j => array index of the room
                                    for (int j : listOfCapacityMatchedRooms) {
                                        this.rooms[j].setRoomTime(hour + l, lessonPlan.getCourseID());    // writing course ID
                                    }

                                    lessonPlan.setCourseTime(hour + l, this.rooms[listOfCapacityMatchedRooms.get(0)].getRoomID());            // writing room ID
                                    this.setStudentsAreNotFree(lessonPlan.getBolumID(), getYearLevel(lessonPlan.getDersKodu()), hour + l, lessonPlan.getCourseID());
                                    this.setTeacherIsNotFree(lessonPlan.getHocaninSicilNumarasi(), hour + l, lessonPlan.getCourseID());
                                }

                                //decreesing the hours of lessons
                                if (lessonType == 0)
                                    lessonPlan.setUnpositionedTeorikHours(lessonPlan.getUnpositionedTeorikHours() - hoursNeeded);
                                if (lessonType == 1)
                                    lessonPlan.setUnpositionedUygulamaHours(lessonPlan.getUnpositionedUygulamaHours() - hoursNeeded);
                            }
                        }
                    }
                }
            }
        }
    }

    private void FindPlaceForTheoryLesson()
    {
        
    }
    
    private void FindPlaceForPracticeLesson()
    {
        
    }
    
    List<int> GenerateRoomsList(Course course, LessonType lessonType, int round) {
        List<int> roomsList = new();
        List<int> capacityDifference = new();

        //If it is the fourth or more round we should find building ID of the Faculty to which current course belongs to.
        int facultyBuildingId = 0;
        if (round >= 4) {
            facultyBuildingId = course.Faculty.BuildingId;
        }

        bool roomTypeMatch, departmentMatch; //, roomCapasityMatch;
        foreach (var j in _classroomsData.AllClassrooms.Keys) {
            var currentRoomType = _classroomsData.AllClassrooms[j].RoomType;
            
            var roomTypeNeeded = lessonType == LessonType.Theory ? course.TheoryRoomType : course.PracticeRoomType;

            roomTypeMatch = RoomTypeCheck(roomTypeNeeded, currentRoomType, round);

            int currentRoomsBuildingId = _classroomsData.AllClassrooms[j].BuildingId;
            if (round <= 3)
                departmentMatch = course.FacultyId == _classroomsData.AllClassrooms[j].Department.FacultyId;        //check if the course and the room are belong to the same faculty
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
                capacityDifference.Add(_classroomsData.AllClassrooms[j].Capacity);    // - course.getMaxOgrenciSayisi());
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
    
    // This function checks if the given course lessons will not devided by lunch time or end of the day
    //In other words: it checks if the lessons will finish before lunch or before end of the day
    bool CheckTimeForLunchOrEndOfDay(int hour, int hoursNeeded)
    {
        return hour % _configuration.HoursPerDay <= _configuration.LunchAfterHour 
            ? hour % _configuration.HoursPerDay + hoursNeeded - 1 <= _configuration.LunchAfterHour 
            : hour % _configuration.HoursPerDay + hoursNeeded - 1 <= _configuration.HoursPerDay - 1;
    }
}
