using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DanceStudioFinder.Models;

[Table("address")]
public partial class Address
{
    [Key]
    [Column("id_address")]
    public int IdAddress { get; set; }

    [Column("entity")]
    [StringLength(30)]
    public string Entity { get; set; } = null!;

    [Column("locality")]
    [StringLength(30)]
    public string Locality { get; set; } = null!;

    [Column("settlement_area")]
    [StringLength(30)]
    public string? SettlementArea { get; set; }

    [Column("street")]
    [StringLength(135)]
    public string Street { get; set; } = null!;

    [Column("building_number")]
    public int BuildingNumber { get; set; }

    [Column("letter")]
    [StringLength(1)]
    public string? Letter { get; set; }

    [InverseProperty("IdAddressNavigation")]
    public virtual ICollection<DanceStudio> DanceStudios { get; set; } = new List<DanceStudio>();
}
