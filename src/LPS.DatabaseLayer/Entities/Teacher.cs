using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LPS.DatabaseLayer.Entities;

[Table("teacher")]
[Index(nameof(UserId), Name = "IDX_B0F6A6D5A76ED395")]
[Index(nameof(Code), Name = "UNIQ_B0F6A6D584D45D6F", IsUnique = true)]
public class Teacher
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    [Column("code")]
    public int Code { get; set; }
    [Required]
    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; }
    [Required]
    [Column("title")]
    [StringLength(255)]
    public string Title { get; set; }
    [Column("extra_data")]
    [StringLength(255)]
    public string ExtraData { get; set; }
    [Column("employee_type")]
    public int EmployeeType { get; set; }
    [Column("archived")]
    public bool Archived { get; set; }
    [Column("user_id")]
    public int? UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    [InverseProperty(nameof(FosUser.Teachers))]
    public virtual FosUser User { get; set; }
    [InverseProperty(nameof(Course.Teacher))]
    public virtual ICollection<Course> Courses { get; set; } = new HashSet<Course>();
}
