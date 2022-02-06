namespace LessonPlanningSystem.DatabaseLayer.Entities;

public class FosUser
{
    public FosUser()
    {
        Classrooms = new HashSet<Classroom>();
        CourseVsRooms = new HashSet<CourseVsRoom>();
        Courses = new HashSet<Course>();
        Teachers = new HashSet<Teacher>();
        TimeTables = new HashSet<TimeTable>();
    }

    public int Id { get; set; }
    public string Username { get; set; }
    public string UsernameCanonical { get; set; }
    public string Email { get; set; }
    public string EmailCanonical { get; set; }
    public bool Enabled { get; set; }
    public string Salt { get; set; }
    public string Password { get; set; }
    public DateTime? LastLogin { get; set; }
    public string ConfirmationToken { get; set; }
    public DateTime? PasswordRequestedAt { get; set; }

    /// <summary>
    ///     (DC2Type:array)
    /// </summary>
    public string Roles { get; set; }

    public string Name { get; set; }
    public int? FacultyId { get; set; }
    public int? PermissionId { get; set; }

    public Faculty Faculty { get; set; }
    public PermissionToEdit Permission { get; set; }
    public ICollection<Classroom> Classrooms { get; set; }
    public ICollection<CourseVsRoom> CourseVsRooms { get; set; }
    public ICollection<Course> Courses { get; set; }
    public ICollection<Teacher> Teachers { get; set; }
    public ICollection<TimeTable> TimeTables { get; set; }
}
