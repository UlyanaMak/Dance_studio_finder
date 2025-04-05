using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DanceStudioFinder.Models;

[Table("week_days")]
public partial class WeekDay
{
    [Key]
    [Column("id_day")]
    public int IdDay { get; set; }

    [Column("short_name")]
    [StringLength(2)]
    public string ShortName { get; set; } = null!;

    [Column("name")]
    [StringLength(11)]
    public string Name { get; set; } = null!;

    [InverseProperty("IdDayNavigation")]
    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
