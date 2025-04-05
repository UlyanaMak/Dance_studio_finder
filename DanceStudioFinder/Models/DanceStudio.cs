using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DanceStudioFinder.Models;

[Table("dance_studio")]
public partial class DanceStudio
{
    [Key]
    [Column("id_studio")]
    public int IdStudio { get; set; }

    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    [Column("id_address")]
    public int IdAddress { get; set; }

    [Column("phone_number")]
    [StringLength(10)]
    public string PhoneNumber { get; set; } = null!;

    [Column("extra_phone_number")]
    [StringLength(10)]
    public string? ExtraPhoneNumber { get; set; }

    [Column("vk_group")]
    [StringLength(255)]
    public string? VkGroup { get; set; }

    [Column("website")]
    [StringLength(255)]
    public string? Website { get; set; }

    [Column("telegram")]
    [StringLength(64)]
    public string? Telegram { get; set; }

    [Column("id_admin")]
    public int IdAdmin { get; set; }

    [InverseProperty("IdStudioNavigation")]
    public virtual ICollection<DanceGroup> DanceGroups { get; set; } = new List<DanceGroup>();

    [ForeignKey("IdAddress")]
    [InverseProperty("DanceStudios")]
    public virtual Address IdAddressNavigation { get; set; } = null!;

    [ForeignKey("IdAdmin")]
    [InverseProperty("DanceStudios")]
    public virtual Admin IdAdminNavigation { get; set; } = null!;

    [InverseProperty("IdStudioNavigation")]
    public virtual ICollection<Price> Prices { get; set; } = new List<Price>();
}
