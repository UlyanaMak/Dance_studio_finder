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
    [Required(ErrorMessage = "Имя - обязательное поле для ввода")]
    [RegularExpression(@"^[А-ЯЁ][а-яё]{0,14}$", ErrorMessage = "Имя должно быть введено на русском языке с заглавной буквы, длина не более 14 символов")]
    public string? Name { get; set; }

    [Column("surname")]
    [StringLength(40)]
    [Required(ErrorMessage = "Фамилия - обязательное поле для ввода")]
    [RegularExpression(@"^[А-ЯЁ][а-яё]{0,39}$", ErrorMessage = "Фамилия должна быть введена на русском языке с заглавной буквы, длина не более 40 символов")]
    public string? Surname { get; set; }

    [Column("email")]
    [StringLength(254)]
    [Required(ErrorMessage = "Эл. почта - обязательное поле для ввода")]
    [EmailAddress(ErrorMessage = "Некорректный формат эл. почты")]
    public string? Email { get; set; }

    [Column("password")]
    [StringLength(128)]
    public string? Password { get; set; }

    [InverseProperty("IdAdminNavigation")]
    public virtual ICollection<DanceStudio> DanceStudios { get; set; } = new List<DanceStudio>();
}
