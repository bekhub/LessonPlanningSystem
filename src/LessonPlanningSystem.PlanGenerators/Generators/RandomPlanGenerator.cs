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

    private void FindPlaceForLesson(Course course, LessonType lessonType, int round)
    {
        var currSgMode = course.SubgroupMode;
        if (currSgMode <= SubgroupMode.Mode2 || currSgMode == SubgroupMode.Mode6 && lessonType == LessonType.Practice) {
            var hoursNeeded = _timetableData.RemainingHoursByLessonType(course, lessonType);

            if (hoursNeeded <= 0) return;

            // checks if there are still unpositioned LESSONS
            // Here specialRequirementsForCourse(i, 0) generates list of special rooms for this type of lessons of this course
            // specialRoomsList is the list of rooms to use.
            var roomList = course.GetRoomIdsForSpecialCourses(lessonType);

            //If there are NOT any special requirements for the given course and this type of lessons
            if (roomList.Count == 0)
                roomList = _classroomsData.GenerateRoomsList(course, lessonType, round);

            if (roomList == null) return;
            foreach (var time in ScheduleTime.GetWeekScheduleTimes()) {
                if (!ScheduleTime.NotLunchOrEndOfDay(time.Hour, hoursNeeded) || 
                    !_timetableData.ScheduleTimeIsFree(course, time, hoursNeeded, round) ||
                    //if(!this.checkHourIsConvenientForCourse(i, hour+hoursNeeded, round)) hourIsConvenientForCourse = false;
                    !course.TimeIsConvenientForCourse(time, round)) continue;
                
                var capacityMatch = false;
                
            }

            for (int hour = 0; hour <= _configuration.WeekHoursNumber - hoursNeeded; hour++) {
                var capacityMatch = false;
                var courseIsFree = true;
                var studentsAreFree = true;
                var teacherIsFree = true;
                var hourIsConvenientForCourse = true;

                //Here we chek if there is enough time until lunch or the end of the day for the hours needed
                if (!this.CheckTimeForLunchOrEndOfDay(hour, hoursNeeded)) continue;
                
                for (int l = 0; l < hoursNeeded; l++) {
                    //System.out.println("ders kodu: "+this.courses[i].getDersKodu()+" hoursNeeded: "+hoursNeeded+" hour: "+hour+" l=: "+l);
                    if (_timetableData.CourseIsFree(course.Id, hour + l) != 0)
                        courseIsFree = false;    //check if the course free at that time (may be the uygulama lesson at the same time but in the other room)

                    // check if the students are free at the given time
                    if (!this.CheckStudentsAreFree(course, course.DepartmentId, course.GradeYear, hour + l, round))
                        studentsAreFree = false;

                    // check if the teacher is free at the given time
                    if (!this.CheckTeacherIsFree(course.Teacher, hour + l))
                        teacherIsFree = false;
                }

                // This function ensures that BZD of 1'st and 3'd level are positioned before noon, and BZD of 2'nd and 4'th level are positioned after noon
                if (!this.CheckHourIsConvenientForCourse(course, hour, round))
                    hourIsConvenientForCourse = false;
                //if(!this.checkHourIsConvenientForCourse(i, hour+hoursNeeded, round)) hourIsConvenientForCourse = false;


                // Let's create a list of free rooms at the given hour
                var listOfFreeRooms = new List<int>();
                //j => array index of the room
                foreach (int j in roomList) {
                    var roomIsFree = true;
                    for (int l = 0; l < hoursNeeded; l++) {
                        try {
                            if (_classroomsData.AllClassrooms[j].GetRoomTime(hour + l) != 0)
                                roomIsFree = false;        //check if the room is free at that time
                        } catch (IndexOutOfRangeException e) {
                            Console.WriteLine("Array Index out of bounds: Mode 1, Uygulama: j=" + j + " hour=" + hour + " course id=" + course.Id);
                        }
                    }
                    if (roomIsFree) listOfFreeRooms.Add(j);
                }

                var listOfCapacityMatchedRooms = new List<int>();
                if (listOfFreeRooms.Count != 0) {
                    listOfCapacityMatchedRooms = this.CheckCapacityMatch(course, listOfFreeRooms, lessonType);
                    if (listOfCapacityMatchedRooms.Count > 0) capacityMatch = true;
                }

                if (capacityMatch && courseIsFree && studentsAreFree && teacherIsFree && hourIsConvenientForCourse) {
                    for (int l = 0; l < hoursNeeded; l++) {
                        //j => array index of the room
                        foreach (int j in listOfCapacityMatchedRooms) {
                            _classroomsData.AllClassrooms[j].SetRoomTime(hour + l, course.Id);    // writing course ID
                        }

                        course.SetCourseTime(hour + l, _classroomsData.AllClassrooms[listOfCapacityMatchedRooms[0]].Id);            // writing room ID
                        this.SetStudentsAreNotFree(course.DepartmentId, course.GradeYear, hour + l, course.Id);
                        this.SetTeacherIsNotFree(course.Teacher, hour + l, course.Id);
                    }
                }
            }
        }
    }

    // This function checks if the given course lessons will not devided by lunch time or end of the day
    //In other words: it checks if the lessons will finish before lunch or before end of the day
    bool CheckTimeForLunchOrEndOfDay(int hour, int hoursNeeded)
    {
        return hour % _configuration.HoursPerDay <= _configuration.LunchAfterHour 
            ? hour % _configuration.HoursPerDay + hoursNeeded - 1 <= _configuration.LunchAfterHour 
            : hour % _configuration.HoursPerDay + hoursNeeded - 1 <= _configuration.HoursPerDay - 1;
    }

    // This function ensures that BZD of 1'st and 3'd level are positioned before noon, and BZD of 2'nd and 4'th level are positioned after noon
    bool CheckHourIsConvenientForCourse(Course course, int hour, int round) {
        // i => index of the course
        //System.out.println("Course: "+this.courses[i].getDersKodu()+" hour: "+hour+" yearLevel: "+this.getYearLevel(this.courses[i].getDersKodu())+" round: "+round);
        // This rule is not acceptable after first round
        if (round < 3) {
            if (course.CourseType == 0) {    // If the course type is BZD
                if (course.GradeYear % 2 == 1) {
                    return hour % _configuration.HoursPerDay <= 3;
                }
                if (course.GradeYear % 2 == 0) {
                    return hour % _configuration.HoursPerDay >= 4;
                }
            }
        }
        return true;    // This rule is not acceptable for the courses other than BZD
    }
    
    //This function checks capacity match for the given list of rooms and returns the list of convenient rooms
    List<int> CheckCapacityMatch(Course course, List<int> listOfFreeRooms, LessonType lessonType) {
        var listOfRooms = new List<int>();

        if (lessonType == 0) {    // If the lesson type is Teorik lesson, then just simple check for capacity of room and course students number
            foreach (int j in listOfFreeRooms) {
                if (course.MaxStudentsNumber <= _classroomsData.AllClassrooms[j].Capacity + 10) {
                    listOfRooms.Add(j);
                    break;
                }
            }
        } else {    // This means if lesson type is Uygulama, then find suitable room, if no room found - try to find two rooms

            // at first let's try single room capacity match
            foreach (int j in listOfFreeRooms) {
                if (course.MaxStudentsNumber <= _classroomsData.AllClassrooms[j].Capacity + 10) {
                    listOfRooms.Add(j);
                    break;
                }
            }

            if (course.PracticeRoomType is RoomType.WithComputers or RoomType.Laboratory) {
                if (listOfRooms.Count == 0) {    // This means that there is not any room with enough capacity. So we need to find two rooms with total capacity that is enough for the course
                    int arrayDimension = listOfFreeRooms.Count;
                    var capacityTotals = new int[arrayDimension, arrayDimension];

                    for (int column = 0; column < arrayDimension; column++) {
                        for (int row = 0; row < arrayDimension; row++) {
                            if (row > column) break;
                            if (row == column)
                                capacityTotals[row, column] = _classroomsData.AllClassrooms[listOfFreeRooms[row]].Capacity;
                            else {
                                capacityTotals[row, column] = _classroomsData.AllClassrooms[listOfFreeRooms[row]].Capacity + _classroomsData.AllClassrooms[listOfFreeRooms[column]].Capacity;
                            }
                        }
                    }

                    bool roomsFound = false;
                    for (int column = 0; column < arrayDimension; column++) {
                        for (int row = 0; row < arrayDimension; row++) {
                            if (row > column) break;
                            if (course.MaxStudentsNumber <= capacityTotals[row, column] + 10) {
                                if (_classroomsData.AllClassrooms[listOfFreeRooms[row]].BuildingId == _classroomsData.AllClassrooms[listOfFreeRooms[column]].BuildingId) {
                                    listOfRooms.Add(listOfFreeRooms[row]);
                                    listOfRooms.Add(listOfFreeRooms[column]);
                                    roomsFound = true;
                                    break;
                                }
                            }
                        }
                        if (roomsFound) break;
                    }
                }

            }
        }
        return listOfRooms;
    }
    
    // This function puts not free flag for studentds of the given year level and department at the given hour
    void SetStudentsAreNotFree(int bolumID, int yearLevel, int hour, int courseID) {
        int j = 0;

        // Search for the desired row
        while (true) {
            if ((this.studentsTimetable[j].getDepartmentID() == bolumID) && (this.studentsTimetable[j].getYearLevel() == yearLevel))
                break;
            else j++;
        }
        this.studentsTimetable[j].setGradeYearTime(hour, courseID);            // Put "not free" flag
        //this.studentsTimetable[j][hour] = j;
    }
    
    //This functions sets "Not free" flag for the given teacher at the given time.
    void SetTeacherIsNotFree(Teacher teacher, int hour, int courseID) {
        teacher.SetTeacherTime(hour, courseID);                                // Set "not free" flag
    }
}
