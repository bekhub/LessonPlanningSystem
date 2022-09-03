using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LPS.DatabaseLayer.Entities;

[Table("fos_user")]
[Index(nameof(FacultyId), Name = "IDX_957A6479680CAB68")]
[Index(nameof(PermissionId), Name = "IDX_957A6479FED90CCA")]
[Index(nameof(UsernameCanonical), Name = "UNIQ_957A647992FC23A8", IsUnique = true)]
[Index(nameof(EmailCanonical), Name = "UNIQ_957A6479A0D96FBF", IsUnique = true)]
[Index(nameof(ConfirmationToken), Name = "UNIQ_957A6479C05FB297", IsUnique = true)]
public class FosUser
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    [Required]
    [Column("username")]
    [StringLength(180)]
    public string Username { get; set; }
    [Required]
    [Column("username_canonical")]
    [StringLength(180)]
    public string UsernameCanonical { get; set; }
    [Required]
    [Column("email")]
    [StringLength(180)]
    public string Email { get; set; }
    [Required]
    [Column("email_canonical")]
    [StringLength(180)]
    public string EmailCanonical { get; set; }
    [Column("enabled")]
    public bool Enabled { get; set; }
    [Column("salt")]
    [StringLength(255)]
    public string Salt { get; set; }
    [Required]
    [Column("password")]
    [StringLength(255)]
    public string Password { get; set; }
    [Column("last_login", TypeName = "datetime")]
    public DateTime? LastLogin { get; set; }
    [Column("confirmation_token")]
    [StringLength(180)]
    public string ConfirmationToken { get; set; }
    [Column("password_requested_at", TypeName = "datetime")]
    public DateTime? PasswordRequestedAt { get; set; }
    /// <summary>
    /// (DC2Type:array)
    /// </summary>
    [Required]
    [Column("roles")]
    public string Roles { get; set; }
    [Required]
    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; }
    [Column("faculty_id")]
    public int? FacultyId { get; set; }
    [Column("permission_id")]
    public int? PermissionId { get; set; }

    [ForeignKey(nameof(FacultyId))]
    [InverseProperty("FosUsers")]
    public virtual Faculty Faculty { get; set; }
    [ForeignKey(nameof(PermissionId))]
    [InverseProperty(nameof(PermissionToEdit.FosUsers))]
    public virtual PermissionToEdit Permission { get; set; }
    [InverseProperty(nameof(Classroom.User))]
    public virtual ICollection<Classroom> Classrooms { get; set; } = new HashSet<Classroom>();

    [InverseProperty(nameof(CourseVsRoom.User))]
    public virtual ICollection<CourseVsRoom> CourseVsRooms { get; set; } = new HashSet<CourseVsRoom>();

    [InverseProperty(nameof(Course.User))]
    public virtual ICollection<Course> Courses { get; set; } = new HashSet<Course>();

    [InverseProperty(nameof(Teacher.User))]
    public virtual ICollection<Teacher> Teachers { get; set; } = new HashSet<Teacher>();

    [InverseProperty(nameof(TimeTablePreview.User))]
    public virtual ICollection<TimeTablePreview> TimeTablePreviews { get; set; } = new HashSet<TimeTablePreview>();

    [InverseProperty(nameof(TimeTable.User))]
    public virtual ICollection<TimeTable> TimeTables { get; set; } = new HashSet<TimeTable>();
}
