using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DanceStudioFinder.Models;

[Table("prices")]
public partial class Price
{
    [Key]
    [Column("id_price")]
    public int IdPrice { get; set; }

    [Column("id_studio")]
    public int IdStudio { get; set; }

    [Column("price")]
    public int Price1 { get; set; }

    [Column("description")]
    [StringLength(10)]
    public string Description { get; set; } = null!;

    [ForeignKey("IdStudio")]
    [InverseProperty("Prices")]
    public virtual DanceStudio IdStudioNavigation { get; set; } = null!;
}
