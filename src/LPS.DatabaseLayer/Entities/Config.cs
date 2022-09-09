using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LPS.DatabaseLayer.Entities;

[Table("config")]
[Index(nameof(Name), Name = "UNIQUE_KEY", IsUnique = true)]
public class Config
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    /// <summary>
    /// Key
    /// </summary>
    [Required]
    [Column("name")]
    [StringLength(512)]
    public string Name { get; set; }
    /// <summary>
    /// Value
    /// </summary>
    [Required]
    [Column("value", TypeName = "text")]
    public string Value { get; set; }
}
