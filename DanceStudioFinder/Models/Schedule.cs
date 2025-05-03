using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DanceStudioFinder.Models;

[Table("schedule")]
public partial class Schedule
{
    [Key]
    [Column("id_schedule")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdSchedule { get; set; }

    [Column("id_group")]
    public int IdGroup { get; set; }

    [Column("id_day")]
    public int IdDay { get; set; }

    [Column("begin_time")]
    public TimeOnly BeginTime { get; set; }

    [Column("end_time")]
    [CustomValidation(typeof(Schedule), nameof(ValidateEndTime))]
    public TimeOnly EndTime { get; set; }

    [ForeignKey("IdDay")]
    [InverseProperty("Schedules")]
    public virtual WeekDay IdDayNavigation { get; set; } = null!;

    [ForeignKey("IdGroup")]
    [InverseProperty("Schedules")]
    public virtual DanceGroup IdGroupNavigation { get; set; } = null!;

    /// <summary>
    /// Валидация времени
    /// </summary>
    /// <param name="endTime"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public static ValidationResult? ValidateEndTime(TimeOnly endTime, ValidationContext context)
    {
        var instance = (Schedule)context.ObjectInstance;
        return endTime > instance.BeginTime? ValidationResult.Success:new ValidationResult("Время окончания должно быть позже времени начала");
    }
}
