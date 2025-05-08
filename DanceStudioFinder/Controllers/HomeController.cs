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

            var styles = _context.Styles.OrderBy(s => s.IdStyle).ToList();  //стили

            var studioFilter = new StudioFilterModel  //представление
            {
                Styles = styles
            };

            var viewModel = new UserViewModel  //созадние модели для главной страницы
            {
                DanceStudios = danceStudios,  //список студий
                StudioFilter = studioFilter   //фильтрация студий
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(StudioFilterModel filter)
        {
            var danceStudios = _context.DanceStudios
                .Include(ds => ds.IdAddressNavigation)
                .Include(ds => ds.Prices)
                .Include(ds => ds.DanceGroups)
                    .ThenInclude(group => group.Schedules)
                .Include(ds => ds.DanceGroups)
                    .ThenInclude(group => group.IdStyleNavigation)
                .AsQueryable();

            // Применяем фильтры
            danceStudios = ApplyFilters(danceStudios, filter);

            var filteredViewModel = new UserViewModel
            {
                DanceStudios = danceStudios.ToList(),
                StudioFilter = filter
            };

            if (!filteredViewModel.DanceStudios.Any())
            {
                ViewBag.NoResultsMessage = "Нет студий, подходящих под заданные фильтры.";
            }

            return View(filteredViewModel);
        }


        private IQueryable<DanceStudio> ApplyFilters(IQueryable<DanceStudio> query, StudioFilterModel filter)
        {
            // Фильтрация по названию
            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                query = query.Where(s => s.Name.Contains(filter.Name));
            }

            // Фильтрация по адресу
            if (!string.IsNullOrWhiteSpace(filter.Locality))
            {
                query = query.Where(s => s.IdAddressNavigation.Locality.Contains(filter.Locality));
            }

            if (!string.IsNullOrWhiteSpace(filter.SettlementArea))
            {
                query = query.Where(s => s.IdAddressNavigation.SettlementArea.Contains(filter.SettlementArea));
            }

            if (!string.IsNullOrWhiteSpace(filter.Street))
            {
                query = query.Where(s => s.IdAddressNavigation.Street.Contains(filter.Street));
            }

            // Фильтрация по стилю - для каждой группы танцев
            if (!string.IsNullOrWhiteSpace(filter.Style))
            {
                var styleId = int.Parse(filter.Style);
                query = query.Where(s => s.DanceGroups.Any(g => g.IdStyle == styleId));
            }

            // Фильтрация по цене
            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(s => s.Prices.Any(p => p.Price1 <= filter.MaxPrice.Value));
            }

            // Фильтрация по времени занятий
            if (filter.BeginTime.HasValue)
            {
                query = query.Where(s => s.DanceGroups.Any(g =>
                    g.Schedules.Any(sc => sc.BeginTime >= filter.BeginTime.Value)));
            }

            if (filter.EndTime.HasValue)
            {
                query = query.Where(s => s.DanceGroups.Any(g =>
                    g.Schedules.Any(sc => sc.EndTime <= filter.EndTime.Value)));
            }

            // Фильтрация по возрасту (если реализовано в модели данных)
            if (filter.Age.HasValue)
            {
                query = query.Where(s => s.DanceGroups.Any(g =>
                    g.IdAgeLimitNavigation.MinAge <= filter.Age.Value &&
                    g.IdAgeLimitNavigation.MaxAge >= filter.Age.Value));
            }

            return query;
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
