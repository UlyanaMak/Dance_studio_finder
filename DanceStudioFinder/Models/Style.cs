using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DanceStudioFinder.Models;

[Table("style")]
public partial class Style
{
    [Key]
    [Column("id_style")]
    public int IdStyle { get; set; }

    [Column("name_eng")]
    [StringLength(30)]
    public string? NameEng { get; set; }

    [Column("name_rus")]
    [StringLength(30)]
    public string? NameRus { get; set; }

    [InverseProperty("IdStyleNavigation")]
    public virtual ICollection<DanceGroup> DanceGroups { get; set; } = new List<DanceGroup>();
}
