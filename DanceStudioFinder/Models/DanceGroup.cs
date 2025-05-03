using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DanceStudioFinder.Models;

[Table("dance_group")]
public partial class DanceGroup
{
    [Key]
    [Column("id_group")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdGroup { get; set; }

    [Column("id_studio")]
    public int IdStudio { get; set; }

    [Column("name")]
    [StringLength(150, ErrorMessage = "Название группы должно быть не более 150 символов в длину\"")]
    [Required(ErrorMessage = "Название группы - обязательное поле для ввода")]
    public string Name { get; set; } = null!;

    [Column("id_style")]
    public int IdStyle { get; set; }

    [Column("id_age_limit")]
    public int IdAgeLimit { get; set; }

    [Column("description")]
    [StringLength(500)]
    public string? Description { get; set; }

    [ForeignKey("IdAgeLimit")]
    [InverseProperty("DanceGroups")]
    public virtual AgeLimit IdAgeLimitNavigation { get; set; } = null!;

    [ForeignKey("IdStudio")]
    [InverseProperty("DanceGroups")]
    public virtual DanceStudio IdStudioNavigation { get; set; } = null!;

    [ForeignKey("IdStyle")]
    [InverseProperty("DanceGroups")]
    public virtual Style IdStyleNavigation { get; set; } = null!;

    [InverseProperty("IdGroupNavigation")]
    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
