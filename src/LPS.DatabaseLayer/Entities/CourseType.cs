using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LPS.DatabaseLayer.Entities;

[Table("course_type")]
public class CourseType
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

    [InverseProperty(nameof(Course.CourseType))]
    public virtual ICollection<Course> Courses { get; set; } = new HashSet<Course>();
}
