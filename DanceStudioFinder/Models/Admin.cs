using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DanceStudioFinder.Models;

[Table("admin")]
public partial class Admin
{
    [Key]
    [Column("id_admin")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdAdmin { get; set; }

    [Column("name")]
    [StringLength(15)]
    public string? Name { get; set; }

    [Column("surname")]
    [StringLength(40)]
    public string? Surname { get; set; }

    [Column("email")]
    [StringLength(254)]
    public string? Email { get; set; }

    [Column("password")]
    [StringLength(128)]
    public string? Password { get; set; }

    [InverseProperty("IdAdminNavigation")]
    public virtual ICollection<DanceStudio> DanceStudios { get; set; } = new List<DanceStudio>();
}
