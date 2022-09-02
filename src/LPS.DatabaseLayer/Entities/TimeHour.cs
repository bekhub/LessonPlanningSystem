using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LPS.DatabaseLayer.Entities;

[Table("time_hour")]
public class TimeHour
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    [Column("order_position")]
    public int? OrderPosition { get; set; }
    [Column("label")]
    [StringLength(45)]
    public string Label { get; set; }

    [InverseProperty(nameof(TimeTablePreview.TimeHour))]
    public virtual ICollection<TimeTablePreview> TimeTablePreviews { get; set; } = new HashSet<TimeTablePreview>();

    [InverseProperty(nameof(TimeTable.TimeHour))]
    public virtual ICollection<TimeTable> TimeTables { get; set; } = new HashSet<TimeTable>();
}
