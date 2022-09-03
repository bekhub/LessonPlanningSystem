using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LPS.DatabaseLayer.Entities;

[Table("lesson_type")]
public class LessonType
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    [Required]
    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; }

    [InverseProperty(nameof(TimeTablePreview.LessonType))]
    public virtual ICollection<TimeTablePreview> TimeTablePreviews { get; set; } = new HashSet<TimeTablePreview>();

    [InverseProperty(nameof(TimeTable.LessonType))]
    public virtual ICollection<TimeTable> TimeTables { get; set; } = new HashSet<TimeTable>();
}
