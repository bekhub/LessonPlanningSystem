using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LPS.DatabaseLayer.Entities;

[Table("course")]
[Index(nameof(AcademicDegreeId), Name = "IDX_169E6FB941807E1D")]
[Index(nameof(CourseTypeId), Name = "IDX_169E6FB95DFA93C1")]
[Index(nameof(UserId), Name = "IDX_169E6FB9A76ED395")]
[Index(nameof(DepartmentId), Name = "IDX_169E6FB9AE80F5DF")]
[Index(nameof(PracticeRoomTypeId), Name = "IDX_169E6FB9E8C99E65")]
[Index(nameof(TheoryRoomTypeId), Name = "IDX_169E6FB9FCD0BC0")]
[Index(nameof(TeacherId), Name = "IDX_8FF59D996804A5EB")]
public class Course
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    [Column("theory_hours")]
    public int TheoryHours { get; set; }
    [Column("practice_hours")]
    public int PracticeHours { get; set; }
    [Column("max_students")]
    public int MaxStudents { get; set; }
    [Required]
    [Column("code")]
    [StringLength(255)]
    public string Code { get; set; }
    [Required]
    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; }
    [Column("credits")]
    public int Credits { get; set; }
    [Required]
    [Column("active")]
    public bool Active { get; set; }
    [Column("subgroup_mode")]
    public int SubgroupMode { get; set; }
    [Column("unpositioned_theory_hours")]
    public int UnpositionedTheoryHours { get; set; }
    [Column("unpositioned_practice_hours")]
    public int UnpositionedPracticeHours { get; set; }
    [Required]
    [Column("semester")]
    [StringLength(255)]
    public string Semester { get; set; }
    [Column("grade_year")]
    public int GradeYear { get; set; }
    [Column("divide_theory_practice")]
    public bool? DivideTheoryPractice { get; set; }
    [Column("academic_degree_id")]
    public int? AcademicDegreeId { get; set; }
    [Column("course_type_id")]
    public int? CourseTypeId { get; set; }
    [Column("department_id")]
    public int? DepartmentId { get; set; }
    [Column("practice_room_type_id")]
    public int? PracticeRoomTypeId { get; set; }
    [Column("teacher_id")]
    public int? TeacherId { get; set; }
    [Column("theory_room_type_id")]
    public int? TheoryRoomTypeId { get; set; }
    [Column("user_id")]
    public int? UserId { get; set; }

    [ForeignKey(nameof(AcademicDegreeId))]
    [InverseProperty("Courses")]
    public virtual AcademicDegree AcademicDegree { get; set; }
    [ForeignKey(nameof(CourseTypeId))]
    [InverseProperty("Courses")]
    public virtual CourseType CourseType { get; set; }
    [ForeignKey(nameof(DepartmentId))]
    [InverseProperty("Courses")]
    public virtual Department Department { get; set; }
    [ForeignKey(nameof(PracticeRoomTypeId))]
    [InverseProperty(nameof(RoomType.CoursePracticeRoomTypes))]
    public virtual RoomType PracticeRoomType { get; set; }
    [ForeignKey(nameof(TeacherId))]
    [InverseProperty("Courses")]
    public virtual Teacher Teacher { get; set; }
    [ForeignKey(nameof(TheoryRoomTypeId))]
    [InverseProperty(nameof(RoomType.CourseTheoryRoomTypes))]
    public virtual RoomType TheoryRoomType { get; set; }
    [ForeignKey(nameof(UserId))]
    [InverseProperty(nameof(FosUser.Courses))]
    public virtual FosUser User { get; set; }
    [InverseProperty(nameof(CourseVsRoom.Course))]
    public virtual ICollection<CourseVsRoom> CourseVsRooms { get; set; } = new HashSet<CourseVsRoom>();

    [InverseProperty(nameof(TimeTablePreview.Course))]
    public virtual ICollection<TimeTablePreview> TimeTablePreviews { get; set; } = new HashSet<TimeTablePreview>();

    [InverseProperty(nameof(TimeTable.Course))]
    public virtual ICollection<TimeTable> TimeTables { get; set; } = new HashSet<TimeTable>();
}
