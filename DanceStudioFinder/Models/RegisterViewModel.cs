using NuGet.Protocol.Plugins;
using System.ComponentModel.DataAnnotations;

namespace DanceStudioFinder.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Имя - обязательное поле для ввода")]
        [RegularExpression(@"^[А-ЯЁ][а-яё]{0,14}$", ErrorMessage ="Имя должно быть введено на русском языке с заглавной буквы, длина не более 14 символов")]
        public string RegisterName { get; set; }

        [Required(ErrorMessage = "Фамилия - обязательное поле для ввода")]
        [RegularExpression(@"^[А-ЯЁ][а-яё]{0,39}$", ErrorMessage = "Фамилия должна быть введена на русском языке с заглавной буквы, длина не более 40 символов")]
        public string RegisterSurname { get; set; }

        [Required(ErrorMessage = "Эл. почта - обязательное поле для ввода")]
        [EmailAddress(ErrorMessage = "Некорректный формат эл. почты")]
        public string RegisterEmail { get; set; }

        [Required(ErrorMessage = "Пароль - обязательное поле для ввода")]
        [StringLength(15, MinimumLength = 5, ErrorMessage = "Пароль должен быть не длинне {1} символов и не короче {2} символов в длину")]
        [DataType(DataType.Password)]
        public string RegisterPassword { get; set; }

        [Required(ErrorMessage = "Повтор пароля - обязательное поле для ввода")]
        [DataType(DataType.Password)]
        [Compare("RegisterPassword", ErrorMessage = "Пароли не совпадают")]
        public string RegisterConfirmPassword { get; set; }

        public Admin GetUser()
        {
            Admin user = new()
            {
                Name = RegisterName,
                Surname = RegisterSurname,
                Email = RegisterEmail,
            };
            return user;
        }
    }
}
