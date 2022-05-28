using LessonPlanningSystem.PlanGenerators.Enums;
using LessonPlanningSystem.PlanGenerators.Models;

namespace LessonPlanningSystem.PlanGenerators.Strategies.Interfaces;

public interface ILessonPlacingStrategy
{
    void FindPlaceForLesson(Course course, LessonType lessonType, Round round);
}
