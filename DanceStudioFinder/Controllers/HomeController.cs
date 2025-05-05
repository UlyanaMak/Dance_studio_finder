using DanceStudioFinder.Data;
using DanceStudioFinder.Models;
using DanceStudioFinder.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Diagnostics;
using System.Security.Claims;

namespace DanceStudioFinder.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAdminService _adminService;
        public HomeController(ApplicationDbContext context, IAdminService adminService)
        {
            _context = context;
            _adminService = adminService;
        }

        public IActionResult Index()
        {
            var danceStudios = _context.DanceStudios
                .Include(ds => ds.IdAddressNavigation) // загрузка адресов
                .Include(ds => ds.Prices)              // загрузка цен
                .ToList();

            var viewModel = new UserViewModel  //созадние модели для главной страницы
            {
                DanceStudios = danceStudios
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(UserViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                ViewData["ModelIsValid"] = true;
                return View();
            }
            ViewData["ModelIsValid"] = false;
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }


        /// <summary>
        /// Регистрация администратора
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserViewModel model)
        {
            ModelState.Remove("Login");  //удалить проверку входа
            ModelState.Remove("DanceStudios");  //удалить проверку танцевальных студий
            
            if (!ModelState.IsValid)  //если не пройдена валидация
            {
                model.DanceStudios = _context.DanceStudios.ToList();  //список студий
                return View("Index", model);  //на главную страницу с модальным окном
            }
            //если пройдена валидация
            try
            {
                Admin admin = new Admin  //создание администратора без пароля
                {
                    Name = model.Register.RegisterName,
                    Surname = model.Register.RegisterSurname,
                    Email = model.Register.RegisterEmail
                };

                var result = await _adminService.RegisterAdmin(admin, model.Register.RegisterPassword);  //регистрация администратора (пароль хэшируется в AdminService)

                if (result)  //если успешно
                {
                    return RedirectToAction("Index", "AdminStudio", new { adminId = admin.IdAdmin });  //перенаправление на страницу студии, привязанной к администратору
                }
                else  //если администратор с такой почтой существует
                {
                    ModelState.AddModelError("Register.RegisterEmail", "Пользователь с данной эл. почтой уже зарегистрирован. Войдите в систему.");  //сообщение
                }
            }
            catch (Exception ex)  //если произошла ошибка
            {
                ModelState.AddModelError(string.Empty, "Произошла ошибка при регистрации. Обратитесь к владельцу сайта.");
            }
            model.DanceStudios = _context.DanceStudios.ToList();  //список всех студий в системе
            return View("Index", model);  //остаётся на главной
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserViewModel model)
        {
            ModelState.Remove("Register");  //удалить проверку регистрации
            ModelState.Remove("DanceStudios");  //удалить проверку танцевальных студий

            if (!ModelState.IsValid)
            {
                model.DanceStudios = _context.DanceStudios.ToList();  //список студий
                return View("Index", model);
            }
            var result = await _adminService.ValidateAdmin(model.Login);
            var admin = await _adminService.GetAdminByEmail(model.Login.LoginEmail);

            if (admin == null)
            {
                ModelState.AddModelError("Login.LoginEmail", "Пользователь с данной эл. почтой ещё не зарегистрирован.");
            }
            else if (result)
            {
                return RedirectToAction("Index", "AdminStudio", new { adminId = admin.IdAdmin });  //перед '=' ДОЛЖНО совпадать с параметром в AdminStudioController Index
            }
            else
            {
                ModelState.AddModelError("Login.LoginPassword", "Пароль некорректен.");
            }

            // Если модель не валидна, возвращаем частичное представление с ошибками
            model.DanceStudios = _context.DanceStudios.ToList();  //список всех студий в системе
            return View("Index", model);  //остаётся на главной
        }
    }
}
