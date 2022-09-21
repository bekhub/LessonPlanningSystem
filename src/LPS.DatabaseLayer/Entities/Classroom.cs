using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LPS.DatabaseLayer.Entities;

[Table("classroom")]
[Index(nameof(BuildingId), Name = "IDX_497D309D4D2A7E12")]
[Index(nameof(UserId), Name = "IDX_497D309DA76ED395")]
[Index(nameof(DepartmentId), Name = "IDX_497D309DAE80F5DF")]
[Index(nameof(RoomTypeId), Name = "IDX_497D309DB28C944D")]
public partial class Classroom
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    [Required]
    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; }
    [Column("capacity")]
    public int Capacity { get; set; }
    [Column("archived")]
    public bool Archived { get; set; }
    [Column("building_id")]
    public int? BuildingId { get; set; }
    [Column("department_id")]
    public int? DepartmentId { get; set; }
    [Column("room_type_id")]
    public int? RoomTypeId { get; set; }
    [Column("user_id")]
    public int? UserId { get; set; }

    [ForeignKey(nameof(BuildingId))]
    [InverseProperty("Classrooms")]
    public virtual Building Building { get; set; }
    [ForeignKey(nameof(DepartmentId))]
    [InverseProperty("Classrooms")]
    public virtual Department Department { get; set; }
    [ForeignKey(nameof(RoomTypeId))]
    [InverseProperty("Classrooms")]
    public virtual RoomType RoomType { get; set; }
    [ForeignKey(nameof(UserId))]
    [InverseProperty(nameof(FosUser.Classrooms))]
    public virtual FosUser User { get; set; }
    [InverseProperty(nameof(CourseVsRoom.Classroom))]
    public virtual ICollection<CourseVsRoom> CourseVsRooms { get; set; } = new HashSet<CourseVsRoom>();

    [InverseProperty(nameof(TimeTablePreview.Classroom))]
    public virtual ICollection<TimeTablePreview> TimeTablePreviews { get; set; } = new HashSet<TimeTablePreview>();

    [InverseProperty(nameof(TimeTable.Classroom))]
    public virtual ICollection<TimeTable> TimeTables { get; set; } = new HashSet<TimeTable>();
}
