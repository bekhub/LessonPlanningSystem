using LessonPlanningSystem.DatabaseLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace LessonPlanningSystem.DatabaseLayer;

public class TimetableV4Context : DbContext
{
    public TimetableV4Context() { }

    public TimetableV4Context(DbContextOptions<TimetableV4Context> options)
        : base(options) { }

    public DbSet<AcademicDegree> AcademicDegrees { get; set; } = null!;
    public DbSet<Building> Buildings { get; set; } = null!;
    public DbSet<Classroom> Classrooms { get; set; } = null!;
    public DbSet<Course> Courses { get; set; } = null!;
    public DbSet<CourseType> CourseTypes { get; set; } = null!;
    public DbSet<CourseVsRoom> CourseVsRooms { get; set; } = null!;
    public DbSet<Department> Departments { get; set; } = null!;
    public DbSet<Faculty> Faculties { get; set; } = null!;
    public DbSet<FosUser> FosUsers { get; set; } = null!;
    public DbSet<GradeYear> GradeYears { get; set; } = null!;
    public DbSet<LessonType> LessonTypes { get; set; } = null!;
    public DbSet<PermissionToEdit> PermissionToEdits { get; set; } = null!;
    public DbSet<RoomType> RoomTypes { get; set; } = null!;
    public DbSet<Teacher> Teachers { get; set; } = null!;
    public DbSet<TimeDay> TimeDays { get; set; } = null!;
    public DbSet<TimeHour> TimeHours { get; set; } = null!;
    public DbSet<TimeTable> TimeTables { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("utf8_general_ci")
            .HasCharSet("utf8");

        modelBuilder.Entity<AcademicDegree>(entity => {
            entity.ToTable("academic_degree");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");

            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Building>(entity => {
            entity.ToTable("building");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");

            entity.Property(e => e.Archived).HasColumnName("archived");

            entity.Property(e => e.DistanceNumber).HasColumnName("distance_number");

            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");

            entity.Property(e => e.ShortName)
                .HasMaxLength(255)
                .HasColumnName("short_name");
        });

        modelBuilder.Entity<Classroom>(entity => {
            entity.ToTable("classroom");

            entity.HasIndex(e => e.BuildingId, "IDX_497D309D4D2A7E12");

            entity.HasIndex(e => e.UserId, "IDX_497D309DA76ED395");

            entity.HasIndex(e => e.DepartmentId, "IDX_497D309DAE80F5DF");

            entity.HasIndex(e => e.RoomTypeId, "IDX_497D309DB28C944D");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Archived).HasColumnName("archived");

            entity.Property(e => e.BuildingId).HasColumnName("building_id");

            entity.Property(e => e.Capacity).HasColumnName("capacity");

            entity.Property(e => e.DepartmentId).HasColumnName("department_id");

            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");

            entity.Property(e => e.RoomTypeId).HasColumnName("room_type_id");

            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Building)
                .WithMany(p => p.Classrooms)
                .HasForeignKey(d => d.BuildingId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_497D309D4D2A7E12");

            entity.HasOne(d => d.Department)
                .WithMany(p => p.Classrooms)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_497D309DAE80F5DF");

            entity.HasOne(d => d.RoomType)
                .WithMany(p => p.Classrooms)
                .HasForeignKey(d => d.RoomTypeId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_497D309DB28C944D");

            entity.HasOne(d => d.User)
                .WithMany(p => p.Classrooms)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_497D309DA76ED395");
        });

        modelBuilder.Entity<Course>(entity => {
            entity.ToTable("course");

            entity.HasIndex(e => e.AcademicDegreeId, "IDX_169E6FB941807E1D");

            entity.HasIndex(e => e.CourseTypeId, "IDX_169E6FB95DFA93C1");

            entity.HasIndex(e => e.UserId, "IDX_169E6FB9A76ED395");

            entity.HasIndex(e => e.DepartmentId, "IDX_169E6FB9AE80F5DF");

            entity.HasIndex(e => e.PracticeRoomTypeId, "IDX_169E6FB9E8C99E65");

            entity.HasIndex(e => e.TheoryRoomTypeId, "IDX_169E6FB9FCD0BC0");

            entity.HasIndex(e => e.TeacherId, "IDX_8FF59D996804A5EB");

            entity.HasIndex(e => e.LessonTypeId, "IDX_D89EAA45E6A8EC53");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.AcademicDegreeId).HasColumnName("academic_degree_id");

            entity.Property(e => e.Active)
                .IsRequired()
                .HasColumnName("active")
                .HasDefaultValueSql("'1'");

            entity.Property(e => e.Archived).HasColumnName("archived");

            entity.Property(e => e.Code)
                .HasMaxLength(255)
                .HasColumnName("code");

            entity.Property(e => e.CourseTypeId).HasColumnName("course_type_id");

            entity.Property(e => e.Credits).HasColumnName("credits");

            entity.Property(e => e.DepartmentId).HasColumnName("department_id");

            entity.Property(e => e.DivideTheoryPractice).HasColumnName("divide_theory_practice");

            entity.Property(e => e.GradeYear).HasColumnName("grade_year");

            entity.Property(e => e.LessonTypeId).HasColumnName("lesson_type_id");

            entity.Property(e => e.MaxStudents).HasColumnName("max_students");

            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");

            entity.Property(e => e.PracticeHours).HasColumnName("practice_hours");

            entity.Property(e => e.PracticeRoomTypeId).HasColumnName("practice_room_type_id");

            entity.Property(e => e.Semester)
                .HasMaxLength(255)
                .HasColumnName("semester");

            entity.Property(e => e.SubgroupMode).HasColumnName("subgroup_mode");

            entity.Property(e => e.TeacherId).HasColumnName("teacher_id");

            entity.Property(e => e.TheoryHours).HasColumnName("theory_hours");

            entity.Property(e => e.TheoryRoomTypeId).HasColumnName("theory_room_type_id");

            entity.Property(e => e.UnpositionedPracticeHours).HasColumnName("unpositioned_practice_hours");

            entity.Property(e => e.UnpositionedTheoryHours).HasColumnName("unpositioned_theory_hours");

            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.AcademicDegree)
                .WithMany(p => p.Courses)
                .HasForeignKey(d => d.AcademicDegreeId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_8FF59D996804A5EB");

            entity.HasOne(d => d.CourseType)
                .WithMany(p => p.Courses)
                .HasForeignKey(d => d.CourseTypeId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_169E6FB95DFA93C1");

            entity.HasOne(d => d.Department)
                .WithMany(p => p.Courses)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_169E6FB9AE80F5DF");

            entity.HasOne(d => d.LessonType)
                .WithMany(p => p.Courses)
                .HasForeignKey(d => d.LessonTypeId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_D89EAA45E6A8EC53");

            entity.HasOne(d => d.PracticeRoomType)
                .WithMany(p => p.CoursePracticeRoomTypes)
                .HasForeignKey(d => d.PracticeRoomTypeId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_169E6FB9E8C99E65");

            entity.HasOne(d => d.Teacher)
                .WithMany(p => p.Courses)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_169E6FB941807E1D");

            entity.HasOne(d => d.TheoryRoomType)
                .WithMany(p => p.CourseTheoryRoomTypes)
                .HasForeignKey(d => d.TheoryRoomTypeId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_169E6FB9FCD0BC0");

            entity.HasOne(d => d.User)
                .WithMany(p => p.Courses)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_169E6FB9A76ED395");
        });

        modelBuilder.Entity<CourseType>(entity => {
            entity.ToTable("course_type");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Archived).HasColumnName("archived");

            entity.Property(e => e.Type)
                .HasMaxLength(255)
                .HasColumnName("type");

            entity.Property(e => e.TypeCode).HasColumnName("type_code");
        });

        modelBuilder.Entity<CourseVsRoom>(entity => {
            entity.ToTable("course_vs_room");

            entity.HasIndex(e => e.CourseId, "IDX_7E399671591CC992");

            entity.HasIndex(e => e.ClassroomId, "IDX_7E3996716278D5A8");

            entity.HasIndex(e => e.UserId, "IDX_7E399671A76ED395");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Archived).HasColumnName("archived");

            entity.Property(e => e.ClassroomId).HasColumnName("classroom_id");

            entity.Property(e => e.CourseId).HasColumnName("course_id");

            entity.Property(e => e.LessonType).HasColumnName("lesson_type");

            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Classroom)
                .WithMany(p => p.CourseVsRooms)
                .HasForeignKey(d => d.ClassroomId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_7E3996716278D5A8");

            entity.HasOne(d => d.Course)
                .WithMany(p => p.CourseVsRooms)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_7E399671591CC992");

            entity.HasOne(d => d.User)
                .WithMany(p => p.CourseVsRooms)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_7E399671A76ED395");
        });

        modelBuilder.Entity<Department>(entity => {
            entity.ToTable("department");

            entity.HasIndex(e => e.FacultyId, "IDX_CD1DE18A680CAB68");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Archived).HasColumnName("archived");

            entity.Property(e => e.FacultyId).HasColumnName("faculty_id");

            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");

            entity.HasOne(d => d.Faculty)
                .WithMany(p => p.Departments)
                .HasForeignKey(d => d.FacultyId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_CD1DE18A680CAB68");
        });

        modelBuilder.Entity<Faculty>(entity => {
            entity.ToTable("faculty");

            entity.HasIndex(e => e.BuildingId, "IDX_179660434D2A7E12");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Archived).HasColumnName("archived");

            entity.Property(e => e.BuildingId).HasColumnName("building_id");

            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");

            entity.HasOne(d => d.Building)
                .WithMany(p => p.Faculties)
                .HasForeignKey(d => d.BuildingId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_179660434D2A7E12");
        });

        modelBuilder.Entity<FosUser>(entity => {
            entity.ToTable("fos_user");

            entity.HasIndex(e => e.FacultyId, "IDX_957A6479680CAB68");

            entity.HasIndex(e => e.PermissionId, "IDX_957A6479FED90CCA");

            entity.HasIndex(e => e.UsernameCanonical, "UNIQ_957A647992FC23A8")
                .IsUnique();

            entity.HasIndex(e => e.EmailCanonical, "UNIQ_957A6479A0D96FBF")
                .IsUnique();

            entity.HasIndex(e => e.ConfirmationToken, "UNIQ_957A6479C05FB297")
                .IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.ConfirmationToken)
                .HasMaxLength(180)
                .HasColumnName("confirmation_token");

            entity.Property(e => e.Email)
                .HasMaxLength(180)
                .HasColumnName("email");

            entity.Property(e => e.EmailCanonical)
                .HasMaxLength(180)
                .HasColumnName("email_canonical");

            entity.Property(e => e.Enabled).HasColumnName("enabled");

            entity.Property(e => e.FacultyId).HasColumnName("faculty_id");

            entity.Property(e => e.LastLogin)
                .HasColumnType("datetime")
                .HasColumnName("last_login");

            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");

            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");

            entity.Property(e => e.PasswordRequestedAt)
                .HasColumnType("datetime")
                .HasColumnName("password_requested_at");

            entity.Property(e => e.PermissionId).HasColumnName("permission_id");

            entity.Property(e => e.Roles)
                .HasColumnName("roles")
                .HasComment("(DC2Type:array)");

            entity.Property(e => e.Salt)
                .HasMaxLength(255)
                .HasColumnName("salt");

            entity.Property(e => e.Username)
                .HasMaxLength(180)
                .HasColumnName("username");

            entity.Property(e => e.UsernameCanonical)
                .HasMaxLength(180)
                .HasColumnName("username_canonical");

            entity.HasOne(d => d.Faculty)
                .WithMany(p => p.FosUsers)
                .HasForeignKey(d => d.FacultyId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_957A6479680CAB68");

            entity.HasOne(d => d.Permission)
                .WithMany(p => p.FosUsers)
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_957A6479FED90CCA");
        });

        modelBuilder.Entity<GradeYear>(entity => {
            entity.ToTable("grade_year");

            entity.HasIndex(e => e.DepartmentId, "IDX_15E03E27AE80F5DF");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Archived).HasColumnName("archived");

            entity.Property(e => e.DepartmentId).HasColumnName("department_id");

            entity.Property(e => e.Grade).HasColumnName("grade");

            entity.HasOne(d => d.Department)
                .WithMany(p => p.GradeYears)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_15E03E27AE80F5DF");
        });

        modelBuilder.Entity<LessonType>(entity => {
            entity.ToTable("lesson_type");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<PermissionToEdit>(entity => {
            entity.ToTable("permission_to_edit");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.End)
                .HasColumnType("datetime")
                .HasColumnName("end");

            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");

            entity.Property(e => e.Semester)
                .HasMaxLength(255)
                .HasColumnName("semester");

            entity.Property(e => e.Start)
                .HasColumnType("datetime")
                .HasColumnName("start");
        });

        modelBuilder.Entity<RoomType>(entity => {
            entity.ToTable("room_type");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Archived).HasColumnName("archived");

            entity.Property(e => e.Type)
                .HasMaxLength(255)
                .HasColumnName("type");

            entity.Property(e => e.TypeCode).HasColumnName("type_code");
        });

        modelBuilder.Entity<Teacher>(entity => {
            entity.ToTable("teacher");

            entity.HasIndex(e => e.UserId, "IDX_B0F6A6D5A76ED395");

            entity.HasIndex(e => e.Code, "UNIQ_B0F6A6D584D45D6F")
                .IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Archived).HasColumnName("archived");

            entity.Property(e => e.Code).HasColumnName("code");

            entity.Property(e => e.EmployeeType).HasColumnName("employee_type");

            entity.Property(e => e.ExtraData)
                .HasMaxLength(255)
                .HasColumnName("extra_data");

            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");

            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User)
                .WithMany(p => p.Teachers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_B0F6A6D5A76ED395");
        });

        modelBuilder.Entity<TimeDay>(entity => {
            entity.ToTable("time_day");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Label)
                .HasMaxLength(45)
                .HasColumnName("label");

            entity.Property(e => e.OrderPosition).HasColumnName("order_position");
        });

        modelBuilder.Entity<TimeHour>(entity => {
            entity.ToTable("time_hour");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Label)
                .HasMaxLength(45)
                .HasColumnName("label");

            entity.Property(e => e.OrderPosition).HasColumnName("order_position");
        });

        modelBuilder.Entity<TimeTable>(entity => {
            entity.ToTable("time_table");

            entity.HasIndex(e => e.TimeDayId, "IDX_173942909876CA92");

            entity.HasIndex(e => e.UserId, "IDX_46301C914D506B7F");

            entity.HasIndex(e => e.TimeHourId, "IDX_659919431397AA98");

            entity.HasIndex(e => e.ClassroomId, "IDX_7B1189B96329C0CC");

            entity.HasIndex(e => e.CourseId, "IDX_88A7A9AF8C63E960");

            entity.HasIndex(e => e.LessonTypeId, "IDX_E11586D37E5B76DE");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.ClassroomId).HasColumnName("classroom_id");

            entity.Property(e => e.CourseId).HasColumnName("course_id");

            entity.Property(e => e.CreatedTime)
                .HasColumnType("datetime")
                .HasColumnName("created_time");

            entity.Property(e => e.EducationalYear)
                .HasMaxLength(9)
                .HasColumnName("educational_year");

            entity.Property(e => e.LessonTypeId).HasColumnName("lesson_type_id");

            entity.Property(e => e.Semester)
                .HasMaxLength(45)
                .HasColumnName("semester");

            entity.Property(e => e.TimeDayId).HasColumnName("time_day_id");

            entity.Property(e => e.TimeHourId).HasColumnName("time_hour_id");

            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Classroom)
                .WithMany(p => p.TimeTables)
                .HasForeignKey(d => d.ClassroomId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_7B1189B96329C0CC");

            entity.HasOne(d => d.Course)
                .WithMany(p => p.TimeTables)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK_88A7A9AF8C63E960");

            entity.HasOne(d => d.LessonType)
                .WithMany(p => p.TimeTables)
                .HasForeignKey(d => d.LessonTypeId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_E11586D37E5B76DE");

            entity.HasOne(d => d.TimeDay)
                .WithMany(p => p.TimeTables)
                .HasForeignKey(d => d.TimeDayId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_173942909876CA92");

            entity.HasOne(d => d.TimeHour)
                .WithMany(p => p.TimeTables)
                .HasForeignKey(d => d.TimeHourId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_659919431397AA98");

            entity.HasOne(d => d.User)
                .WithMany(p => p.TimeTables)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_46301C914D506B7F");
        });
    }
}
