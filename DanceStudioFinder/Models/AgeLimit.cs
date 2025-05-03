using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DanceStudioFinder.Models;

[Table("age_limit")]
public partial class AgeLimit
{
    [Key]
    [Column("id_age_limit")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdAgeLimit { get; set; }

    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Column("max_age")]
    public int? MaxAge { get; set; }

    [Column("min_age")]
    public int? MinAge { get; set; }

    [InverseProperty("IdAgeLimitNavigation")]
    public virtual ICollection<DanceGroup> DanceGroups { get; set; } = new List<DanceGroup>();
}
