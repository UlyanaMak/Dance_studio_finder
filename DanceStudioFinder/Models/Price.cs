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
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdPrice { get; set; }

    [Column("id_studio")]
    public int IdStudio { get; set; }

    [Column("price")]
    [Range(0, 100000, ErrorMessage = "Стоимость занятия должна быть от 0 - бесплатно - до 100 000 рублей")]
    [Required(ErrorMessage = "Цена - поле обязательное для заполнения")]
    public int Price1 { get; set; }

    [Column("description")]
    [StringLength(500, ErrorMessage = "Описание типа цены не должно превышать {1} символов")]
    [Required(ErrorMessage = "Описание цены - поле обязательное для заполнения")]
    public string Description { get; set; } = null!;

    [ForeignKey("IdStudio")]
    [InverseProperty("Prices")]
    public virtual DanceStudio IdStudioNavigation { get; set; } = null!;
}
