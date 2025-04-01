using System.ComponentModel.DataAnnotations;

namespace DanceStudioFinder.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Эл. почта - обязательное поле для ввода")]
        [EmailAddress]
        public string LoginEmail { get; set; }

        [Required(ErrorMessage = "Пароль - обязательное поле для ввода")]
        [DataType(DataType.Password)]
        public string LoginPassword { get; set; }


    }
}
