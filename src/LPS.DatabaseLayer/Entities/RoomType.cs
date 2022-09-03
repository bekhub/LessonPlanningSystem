using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LPS.DatabaseLayer.Entities;

[Table("room_type")]
public class RoomType
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    [Required]
    [Column("type")]
    [StringLength(255)]
    public string Type { get; set; }
    [Column("type_code")]
    public int TypeCode { get; set; }
    [Column("archived")]
    public bool Archived { get; set; }

    [InverseProperty(nameof(Classroom.RoomType))]
    public virtual ICollection<Classroom> Classrooms { get; set; } = new HashSet<Classroom>();

    [InverseProperty(nameof(Course.PracticeRoomType))]
    public virtual ICollection<Course> CoursePracticeRoomTypes { get; set; } = new HashSet<Course>();

    [InverseProperty(nameof(Course.TheoryRoomType))]
    public virtual ICollection<Course> CourseTheoryRoomTypes { get; set; } = new HashSet<Course>();
}
