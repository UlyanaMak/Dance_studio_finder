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
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdAddress { get; set; }

    [Column("entity")]
    [StringLength(30, ErrorMessage = "Название субъекта РФ не более 30 символов")]
    [RegularExpression(@"^[А-Яа-яЁё\s\-]+$", ErrorMessage = "В названии субъекта РФ разрешены только русские буквы")]
    [Required(ErrorMessage = "Субъект РФ - поле обязательное для заполнения")]
    public string Entity { get; set; } = null!;

    [Column("locality")]
    [StringLength(30, ErrorMessage = "Название населённого пункта не более 30 символов")]
    [RegularExpression(@"^[А-Яа-яЁё\s\-]+$", ErrorMessage = "В названии населённого пункта разрешены только русские буквы")]
    [Required(ErrorMessage = "Населённый пункт - поле обязательное для заполнения")]
    public string Locality { get; set; } = null!;

    [Column("settlement_area")]
    [StringLength(30)]
    public string? SettlementArea { get; set; }

    [Column("street")]
    [StringLength(135, ErrorMessage = "Название улицы не более 135 символов")]
    [RegularExpression(@"^[А-Яа-яЁё\s\-]+$", ErrorMessage = "В названии улицы разрешены только русские буквы")]
    [Required(ErrorMessage = "Улица - поле обязательное для заполнения")]
    public string Street { get; set; } = null!;

    [Column("building_number")]
    [Required(ErrorMessage = "Номер здания - поле обязательное для заполнения")]
    [Range(1, 1000, ErrorMessage = "Номер здания должен быть от 1 до 1000")]
    public int BuildingNumber { get; set; }

    [Column("letter")]
    [RegularExpression("^[А-Яа-яA-Za-z]$", ErrorMessage = "В литере здания допускается только одна буква")]
    [StringLength(1, ErrorMessage = "Литера может содержать только один символ")]
    public string? Letter { get; set; }

    [InverseProperty("IdAddressNavigation")]
    public virtual ICollection<DanceStudio> DanceStudios { get; set; } = new List<DanceStudio>();
}
