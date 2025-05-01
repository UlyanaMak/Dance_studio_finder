﻿using System;
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
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdStudio { get; set; }

    [Column("name")]
    [StringLength(255, ErrorMessage = "Название студии должно быть не более {1} символов в длину")]
    [Required(ErrorMessage = "Название студии - обязательное поле для ввода")]
    public string Name { get; set; } = null!;

    [Column("id_address")]
    public int IdAddress { get; set; }

    [Column("phone_number")]
    [StringLength(10, MinimumLength = 10, ErrorMessage ="Длина номера телефона - 10 символов")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Телефон должен содержать только 10 цифр")]
    [Required(ErrorMessage = "Номер телефона - обязательное поле для ввода")]
    public string PhoneNumber { get; set; } = null!;

    [Column("extra_phone_number")]
    [StringLength(10, MinimumLength = 10, ErrorMessage = "Длина номера телефона - 10 символов")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Телефон должен содержать ровно 10 цифр")]
    public string? ExtraPhoneNumber { get; set; }

    [Column("vk_group")]
    [StringLength(255, ErrorMessage ="Ссылка на соц. сеть ВКонтакте не более {1} символов в длину")]
    public string? VkGroup { get; set; }

    [Column("website")]
    [StringLength(255, ErrorMessage = "Ссылка на сайт студии не более {1} символов в длину")]
    public string? Website { get; set; }

    [Column("telegram")]
    [StringLength(64, ErrorMessage = "Ссылка на соц. сеть Telegram не более {1} символов в длину")]
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
