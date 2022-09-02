using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LPS.DatabaseLayer.Entities;

[Table("building")]
public class Building
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    [Required]
    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; }
    [Required]
    [Column("address")]
    [StringLength(255)]
    public string Address { get; set; }
    [Required]
    [Column("short_name")]
    [StringLength(255)]
    public string ShortName { get; set; }
    [Column("distance_number")]
    public int DistanceNumber { get; set; }
    [Column("archived")]
    public bool Archived { get; set; }

    [InverseProperty(nameof(Classroom.Building))]
    public virtual ICollection<Classroom> Classrooms { get; set; } = new HashSet<Classroom>();

    [InverseProperty(nameof(Faculty.Building))]
    public virtual ICollection<Faculty> Faculties { get; set; } = new HashSet<Faculty>();
}
