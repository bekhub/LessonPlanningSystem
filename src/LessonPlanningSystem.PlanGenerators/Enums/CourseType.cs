namespace LessonPlanningSystem.PlanGenerators.Enums;

public enum CourseType
{
    /// <summary>
    /// Bölüm Zorunlu Ders (BZD)
    /// </summary>
    DepartmentMandatory = 0,
    /// <summary>
    /// Ortak Zorunlu Ders (OZD)
    /// </summary>
    GeneralMandatory = 1,
    /// <summary>
    /// Bölüm İçi Seçmeli Ders (BİSD)
    /// </summary>
    DepartmentElective = 2,
    /// <summary>
    /// Bölüm Dışı Seçmeli Ders (BDSD)
    /// </summary>
    NonDepartmentalElective = 3,
    /// <summary>
    /// Uzaktan Eğitim Ders (UED)
    /// </summary>
    RemoteEducation = 4,
}
