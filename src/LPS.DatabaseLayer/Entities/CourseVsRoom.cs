using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LPS.DatabaseLayer.Entities;

[Table("course_vs_room")]
[Index(nameof(CourseId), Name = "IDX_7E399671591CC992")]
[Index(nameof(ClassroomId), Name = "IDX_7E3996716278D5A8")]
[Index(nameof(UserId), Name = "IDX_7E399671A76ED395")]
public class CourseVsRoom
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    [Column("lesson_type")]
    public int LessonType { get; set; }
    [Column("archived")]
    public bool Archived { get; set; }
    [Column("classroom_id")]
    public int? ClassroomId { get; set; }
    [Column("course_id")]
    public int? CourseId { get; set; }
    [Column("user_id")]
    public int? UserId { get; set; }

    [ForeignKey(nameof(ClassroomId))]
    [InverseProperty("CourseVsRooms")]
    public virtual Classroom Classroom { get; set; }
    [ForeignKey(nameof(CourseId))]
    [InverseProperty("CourseVsRooms")]
    public virtual Course Course { get; set; }
    [ForeignKey(nameof(UserId))]
    [InverseProperty(nameof(FosUser.CourseVsRooms))]
    public virtual FosUser User { get; set; }
}
