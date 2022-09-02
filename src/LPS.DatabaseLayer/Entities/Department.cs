using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LPS.DatabaseLayer.Entities;

[Table("department")]
[Index(nameof(FacultyId), Name = "IDX_CD1DE18A680CAB68")]
public class Department
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    [Required]
    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; }
    [Column("archived")]
    public bool Archived { get; set; }
    [Column("faculty_id")]
    public int? FacultyId { get; set; }

    [ForeignKey(nameof(FacultyId))]
    [InverseProperty("Departments")]
    public virtual Faculty Faculty { get; set; }
    [InverseProperty(nameof(Classroom.Department))]
    public virtual ICollection<Classroom> Classrooms { get; set; } = new HashSet<Classroom>();

    [InverseProperty(nameof(Course.Department))]
    public virtual ICollection<Course> Courses { get; set; } = new HashSet<Course>();

    [InverseProperty(nameof(GradeYear.Department))]
    public virtual ICollection<GradeYear> GradeYears { get; set; } = new HashSet<GradeYear>();
}
