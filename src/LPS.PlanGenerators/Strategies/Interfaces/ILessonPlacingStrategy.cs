using LPS.PlanGenerators.Enums;
using LPS.PlanGenerators.Models;

namespace LPS.PlanGenerators.Strategies.Interfaces;

public interface ILessonPlacingStrategy
{
    void FindPlaceForLesson(Course course, LessonType lessonType, Round round);
}
