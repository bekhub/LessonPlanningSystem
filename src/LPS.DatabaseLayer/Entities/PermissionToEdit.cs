using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LPS.DatabaseLayer.Entities;

[Table("permission_to_edit")]
public class PermissionToEdit
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    [Required]
    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; }
    [Column("start", TypeName = "datetime")]
    public DateTime Start { get; set; }
    [Column("end", TypeName = "datetime")]
    public DateTime End { get; set; }
    [Required]
    [Column("semester")]
    [StringLength(255)]
    public string Semester { get; set; }

    [InverseProperty(nameof(FosUser.Permission))]
    public virtual ICollection<FosUser> FosUsers { get; set; } = new HashSet<FosUser>();
}
