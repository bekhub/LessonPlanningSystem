using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LPS.DatabaseLayer.Entities;

[Table("academic_degree")]
public class AcademicDegree
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    [Required]
    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; }

    [InverseProperty(nameof(Course.AcademicDegree))]
    public virtual ICollection<Course> Courses { get; set; } = new HashSet<Course>();
}
