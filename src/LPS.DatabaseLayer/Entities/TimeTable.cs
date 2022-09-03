﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LPS.DatabaseLayer.Entities;

[Table("time_table")]
[Index(nameof(TimeDayId), Name = "IDX_173942909876CA92")]
[Index(nameof(UserId), Name = "IDX_46301C914D506B7F")]
[Index(nameof(TimeHourId), Name = "IDX_659919431397AA98")]
[Index(nameof(ClassroomId), Name = "IDX_7B1189B96329C0CC")]
[Index(nameof(CourseId), Name = "IDX_88A7A9AF8C63E960")]
[Index(nameof(LessonTypeId), Name = "IDX_E11586D37E5B76DE")]
public class TimeTable
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    [Required]
    [Column("educational_year")]
    [StringLength(9)]
    public string EducationalYear { get; set; }
    [Column("semester")]
    [StringLength(45)]
    public string Semester { get; set; }
    [Column("created_time", TypeName = "datetime")]
    public DateTime CreatedTime { get; set; }
    [Column("classroom_id")]
    public int? ClassroomId { get; set; }
    [Column("course_id")]
    public int CourseId { get; set; }
    [Column("lesson_type_id")]
    public int? LessonTypeId { get; set; }
    [Column("time_day_id")]
    public int? TimeDayId { get; set; }
    [Column("time_hour_id")]
    public int? TimeHourId { get; set; }
    [Column("user_id")]
    public int? UserId { get; set; }

    [ForeignKey(nameof(ClassroomId))]
    [InverseProperty("TimeTables")]
    public virtual Classroom Classroom { get; set; }
    [ForeignKey(nameof(CourseId))]
    [InverseProperty("TimeTables")]
    public virtual Course Course { get; set; }
    [ForeignKey(nameof(LessonTypeId))]
    [InverseProperty("TimeTables")]
    public virtual LessonType LessonType { get; set; }
    [ForeignKey(nameof(TimeDayId))]
    [InverseProperty("TimeTables")]
    public virtual TimeDay TimeDay { get; set; }
    [ForeignKey(nameof(TimeHourId))]
    [InverseProperty("TimeTables")]
    public virtual TimeHour TimeHour { get; set; }
    [ForeignKey(nameof(UserId))]
    [InverseProperty(nameof(FosUser.TimeTables))]
    public virtual FosUser User { get; set; }
}
