using System.ComponentModel.DataAnnotations;

namespace DanceStudioFinder.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Имя - обязательное поле для ввода")]
        public string RegisterName { get; set; }

        [Required(ErrorMessage = "Фамилия - обязательное поле для ввода")]
        public string RegisterSurname { get; set; }

        [Required(ErrorMessage = "Эл. почта - обязательное поле для ввода")]
        [EmailAddress]
        public string RegisterEmail { get; set; }

        [Required(ErrorMessage = "Пароль - обязательное поле для ввода")]
        [StringLength(15, MinimumLength = 5, ErrorMessage = "Пароль должен быть не длинне {1} символов и не короче {2} символов в длину")]
        [DataType(DataType.Password)]
        [Compare("RegisterConfirmPassword", ErrorMessage = "Пароли не совпадают")]
        public string RegisterPassword { get; set; }

        [Required(ErrorMessage = "Повтор пароля - обязательное поле для ввода")]
        [DataType(DataType.Password)]
        public string RegisterConfirmPassword { get; set; }

    }
}
