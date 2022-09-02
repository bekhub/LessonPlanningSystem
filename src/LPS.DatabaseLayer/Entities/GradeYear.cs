using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LPS.DatabaseLayer.Entities;

[Table("grade_year")]
[Index(nameof(DepartmentId), Name = "IDX_15E03E27AE80F5DF")]
public class GradeYear
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    [Column("grade")]
    public int Grade { get; set; }
    [Column("archived")]
    public bool Archived { get; set; }
    [Column("department_id")]
    public int? DepartmentId { get; set; }

    [ForeignKey(nameof(DepartmentId))]
    [InverseProperty("GradeYears")]
    public virtual Department Department { get; set; }
}
