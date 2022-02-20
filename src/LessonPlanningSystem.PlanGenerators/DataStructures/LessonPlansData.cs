using LessonPlanningSystem.PlanGenerators.Models;

namespace LessonPlanningSystem.PlanGenerators.DataStructures;

public class LessonPlansData
{
    private readonly Dictionary<int, LessonPlan> _allLessonPlans;

    /// <summary>
    /// By course id
    /// </summary>
    public IReadOnlyDictionary<int, LessonPlan> AlLessonPlans => _allLessonPlans;

    public Dictionary<(int DepartmentId, int GradeYear), List<LessonPlan>> StudentsLessonPlans { get; set; }
    
    // This function checks if the studentds of the given year level and department are free at the given hour
    bool CheckStudentsAreFree(LessonPlan courseIndex, int departmentId, int yearLevel, int hour, int round) { 
        int j = 0;

        var courseIds = StudentsLessonPlans[(departmentId, yearLevel)];

        var lessonPlan = AlLessonPlans.Values.Where(x =>
            x.Course.DepartmentId == departmentId && x.Course.GradeYear == yearLevel);
        
        // Search for the desired row
        try {
            foreach (var lessonPlan in AlLessonPlans.Values) {
                if (lessonPlan.Course.DepartmentId == departmentId && lessonPlan.Course.GradeYear == yearLevel) break;
            }
            while (true) {
                if ((this.studentsTimetable[j].getDepartmentID() == departmentId) && (this.studentsTimetable[j].getYearLevel() == yearLevel))
                    break;
                else j++;
            }
        } catch (IndexOutOfRangeException e) {
            Console.WriteLine("Exception from checkStudentsAreFree: department" + departmentId + " year level: " + yearLevel + " courseIndex: " + courseIndex + " error: " + e);
        }

        // If students are free - return true! If students are not free, then check the course type. 
        // If course type at given time is BISD and the type of the course to put is also BISD - return true (can put BISD courses at the same time)
        // Otherwise return false;
        int courseID = this.studentsTimetable[j].getGradeYearTime(hour);

        try {
            if (courseID == BOOKED) return false;
            if (courseID == 0) return true;
            return (this.courses[this.findCourseIndexByID(courseID)].getCourseType() == this.courses[courseIndex].getCourseType()) && (this.courses[courseIndex].getCourseType() == 2) && (round > 1);
        } catch (IndexOutOfRangeException e) {
            Console.WriteLine("Exception from checkStudentsAreFree: department" + departmentId + " year level: " + yearLevel + " courseIndex: " + courseIndex + " Found course ID: " + courseID + " course index: " + this.findCourseIndexByID(courseID) + " error: " + e);
            throw;
        }
    }
}
