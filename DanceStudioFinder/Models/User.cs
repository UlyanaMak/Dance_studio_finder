using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace DanceStudioFinder.Models
{
    public class User : IdentityUser
    {
        string name;
        string surname;
        string email;
        string password;
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name
        {
            get => name;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Exception("Имя - обязательное для заполнения поле.");
                string pattern = @"^[А-ЯЁ][а-яё]{0,14}$";
                if (!Regex.IsMatch(value, pattern))
                    throw new ArgumentException("Ошибка ввода имени.");
                else
                    name = value;
            }
        }
        public string Surname
        {
            get => surname;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Exception("Фамилия - обязательное для заполнения поле.");
                /*else if (value.Length > 40)
                    throw new Exception("Длина фамилии не больше 40 символов.");*/
                string pattern = @"^[А-ЯЁ][а-яё]{0,39}$";
                if (!Regex.IsMatch(value, pattern))
                    throw new ArgumentException("Ошибка ввода фамилии.");
                else
                    surname = value;
            }
        }
        public string Email
        {
            get => email;
            set
            {
                string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]{2,}$";
                if (string.IsNullOrEmpty(value))
                    throw new Exception("Эл. почта - обязательное для заполнения поле.");

                else if (!Regex.IsMatch(value, pattern))
                    throw new Exception("Эл. почта введена некорректно.");
                else
                    email = value;
            }
        }
        public string Password
        {
            get => password;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Exception("Пароль - обязательное для заполнения поле.");
                else if (value == "DA39A3EE5E6B4B0D3255BFEF95601890AFD80709")          //проверка на пустоту
                    throw new Exception("Пароль - обязательное для заполнения поле.");
                else
                    password = value;
            }
        }

        public static string GetPasswordHash(string password)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] sourceBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha1.ComputeHash(sourceBytes);
                string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
                return hash;
            }
        }

        public bool IsAdmin { get; set; }

        public User()
        {
            IsAdmin = false;
        }

        public User(string name, string surname, string email, string password)
        {
            Name = name;
            Surname = surname;
            Email = email;
            Password = password;
            IsAdmin = true;
        }
    }
}
