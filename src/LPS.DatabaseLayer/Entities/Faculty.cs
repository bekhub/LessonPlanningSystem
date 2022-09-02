using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LPS.DatabaseLayer.Entities;

[Table("faculty")]
[Index(nameof(BuildingId), Name = "IDX_179660434D2A7E12")]
public class Faculty
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
    [Column("building_id")]
    public int? BuildingId { get; set; }

    [ForeignKey(nameof(BuildingId))]
    [InverseProperty("Faculties")]
    public virtual Building Building { get; set; }
    [InverseProperty(nameof(Department.Faculty))]
    public virtual ICollection<Department> Departments { get; set; } = new HashSet<Department>();

    [InverseProperty(nameof(FosUser.Faculty))]
    public virtual ICollection<FosUser> FosUsers { get; set; } = new HashSet<FosUser>();
}
