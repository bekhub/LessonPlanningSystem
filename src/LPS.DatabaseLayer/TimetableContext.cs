using Microsoft.EntityFrameworkCore;
using LPS.DatabaseLayer.Entities;

namespace LPS.DatabaseLayer;

public class TimetableContext : DbContext
{
    public TimetableContext()
    {
    }

    public TimetableContext(DbContextOptions<TimetableContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AcademicDegree> AcademicDegrees { get; set; }
    public virtual DbSet<Building> Buildings { get; set; }
    public virtual DbSet<Classroom> Classrooms { get; set; }
    public virtual DbSet<Config> Configs { get; set; }
    public virtual DbSet<Course> Courses { get; set; }
    public virtual DbSet<CourseType> CourseTypes { get; set; }
    public virtual DbSet<CourseVsRoom> CourseVsRooms { get; set; }
    public virtual DbSet<Department> Departments { get; set; }
    public virtual DbSet<Faculty> Faculties { get; set; }
    public virtual DbSet<FosUser> FosUsers { get; set; }
    public virtual DbSet<GradeYear> GradeYears { get; set; }
    public virtual DbSet<LessonType> LessonTypes { get; set; }
    public virtual DbSet<PermissionToEdit> PermissionToEdits { get; set; }
    public virtual DbSet<RoomType> RoomTypes { get; set; }
    public virtual DbSet<Teacher> Teachers { get; set; }
    public virtual DbSet<TimeDay> TimeDays { get; set; }
    public virtual DbSet<TimeHour> TimeHours { get; set; }
    public virtual DbSet<TimeTable> TimeTables { get; set; }
    public virtual DbSet<TimeTablePreview> TimeTablePreviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("utf8_general_ci")
            .HasCharSet("utf8");

        modelBuilder.Entity<AcademicDegree>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Classroom>(entity =>
        {
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

        modelBuilder.Entity<Config>(entity =>
        {
            entity.Property(e => e.Name).HasComment("Key");

            entity.Property(e => e.Value).HasComment("Value");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.Property(e => e.Active).HasDefaultValueSql("'1'");

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

        modelBuilder.Entity<CourseVsRoom>(entity =>
        {
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

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasOne(d => d.Faculty)
                .WithMany(p => p.Departments)
                .HasForeignKey(d => d.FacultyId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_CD1DE18A680CAB68");
        });

        modelBuilder.Entity<Faculty>(entity =>
        {
            entity.HasOne(d => d.Building)
                .WithMany(p => p.Faculties)
                .HasForeignKey(d => d.BuildingId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_179660434D2A7E12");
        });

        modelBuilder.Entity<FosUser>(entity =>
        {
            entity.Property(e => e.Roles).HasComment("(DC2Type:array)");

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

        modelBuilder.Entity<GradeYear>(entity =>
        {
            entity.HasOne(d => d.Department)
                .WithMany(p => p.GradeYears)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_15E03E27AE80F5DF");
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasOne(d => d.User)
                .WithMany(p => p.Teachers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_B0F6A6D5A76ED395");
        });

        modelBuilder.Entity<TimeTable>(entity =>
        {
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

        modelBuilder.Entity<TimeTablePreview>(entity =>
        {
            entity.HasOne(d => d.Classroom)
                .WithMany(p => p.TimeTablePreviews)
                .HasForeignKey(d => d.ClassroomId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_7B1189B96Preview");

            entity.HasOne(d => d.Course)
                .WithMany(p => p.TimeTablePreviews)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK_88A7A9AF8Preview");

            entity.HasOne(d => d.LessonType)
                .WithMany(p => p.TimeTablePreviews)
                .HasForeignKey(d => d.LessonTypeId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_E11586D37Preview");

            entity.HasOne(d => d.TimeDay)
                .WithMany(p => p.TimeTablePreviews)
                .HasForeignKey(d => d.TimeDayId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_173942909Preview");

            entity.HasOne(d => d.TimeHour)
                .WithMany(p => p.TimeTablePreviews)
                .HasForeignKey(d => d.TimeHourId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_659919431Preview");

            entity.HasOne(d => d.User)
                .WithMany(p => p.TimeTablePreviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_46301C914Preview");
        });
    }
}
