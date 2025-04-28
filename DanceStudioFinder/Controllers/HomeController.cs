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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserViewModel model)
        {
            ModelState.Remove("Login");  //удалить проверку входа
            ModelState.Remove("DanceStudios");  //удалить проверку танцевальных студий
            
            if (!ModelState.IsValid)  //если не пройдена валидация
            {
                model.DanceStudios = _context.DanceStudios.ToList();
                return View("Index", model);
            }
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
                    return RedirectToAction("Index", "AddStudio");  //перенаправление на страницу добавления студии
                }
                else  // Если пользователь с такой почтой существует
                {
                    ModelState.AddModelError("Register.RegisterEmail", "Пользователь с данной эл. почтой уже зарегистрирован");  // Сообщаем об этом пользователю
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Произошла ошибка при регистрации. Обратитесь к администратору.");
            }
            model.DanceStudios = _context.DanceStudios.ToList();
            return View("Index", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = new UserViewModel
                {
                    DanceStudios = _context.DanceStudios.ToList(),
                    Login = model
                };
                return View("Index", viewModel);
            }
            /*if (ModelState.IsValid)
            {
                if (await _adminService.ValidateAdmin(model))
                {
                    var admin = await _adminService.GetAdminByEmail(model.LoginEmail);

                    // Создаем Claims (утверждения) администратора
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, admin.Name),

                    new Claim(ClaimTypes.Email, admin.Email)
                        // Добавьте другие claims, если нужно
                };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme); Scheme, claimsPrincipal, authProperties);

                    // Успешный вход, перенаправляем на главную страницу
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Неверный логин или пароль.");
                    return PartialView("_LoginModal", model);  // Возвращаем модальное окно с ошибками
                }
            }*/

            // Если модель не валидна, возвращаем частичное представление с ошибками
            return PartialView("_LoginModal", model);
        }
    }
}
