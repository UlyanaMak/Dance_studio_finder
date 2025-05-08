using DanceStudioFinder.Data;
using DanceStudioFinder.Models;
using DanceStudioFinder.Services;
using DanceStudioFinder.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DanceStudioFinder.Controllers
{
    public class InfoAdminStudioController : Controller
    {
        private readonly InfoAdminStudioService _infoAdminStudioService;
        private readonly AdminStudioService _adminStudioService;
        private readonly ApplicationDbContext _context;
        public InfoAdminStudioController(InfoAdminStudioService infoAdminStudioService, 
            AdminStudioService adminStudioService, 
            ApplicationDbContext context)
        {
            _infoAdminStudioService = infoAdminStudioService;
            _adminStudioService = adminStudioService;
            _context = context;
        }
        public async Task<IActionResult> Index(int adminId)
        {
            // Находим студию администратора со всеми зависимостями
            var studio = await _context.DanceStudios
                .Include(s => s.IdAdminNavigation) // Данные администратора
                .Include(s => s.IdAddressNavigation) // Адрес студии
                .Include(s => s.Prices) // Цены
                .Include(s => s.DanceGroups) // Группы
                    .ThenInclude(g => g.IdStyleNavigation) // Стили групп
                .Include(s => s.DanceGroups)
                    .ThenInclude(g => g.IdAgeLimitNavigation) // Возрастные ограничения
                .Include(s => s.DanceGroups)
                    .ThenInclude(g => g.Schedules) // Расписание
                        .ThenInclude(s => s.IdDayNavigation) // Дни недели
                .FirstOrDefaultAsync(s => s.IdAdmin == adminId); // Фильтр по ID администратора

            if (studio?.IdAdminNavigation == null)
            {
                return NotFound("Студия для данного администратора не найдена");
            }

            // Формируем ViewModel
            var viewModel = new AdminStudioDetailsViewModel
            {
                Admin = studio.IdAdminNavigation,
                Studio = studio,
                Groups = studio.DanceGroups.Select(g => new AdminDanceGroupViewModel
                {
                    IdGroup = g.IdGroup,
                    Name = g.Name,
                    Description = g.Description,
                    Style = g.IdStyleNavigation,
                    AgeLimit = g.IdAgeLimitNavigation,
                    Schedule = g.Schedules.Select(s => new AdminScheduleDisplayModel
                    {
                        IdSchedule = s.IdSchedule,
                        Day = s.IdDayNavigation,
                        BeginTime = s.BeginTime,
                        EndTime = s.EndTime
                    }).ToList()
                }).ToList(),
                Prices = studio.Prices.ToList(),
                Styles = await _context.Styles.OrderBy(s => s.IdStyle).ToListAsync(),
                WeekDays = await _context.WeekDays.ToListAsync()
            };

            return View(viewModel);
        }


        /// <summary>
        /// Удаление студии
        /// </summary>
        /// <param name="adminId"></param>
        /// <returns></returns>
        public async Task<IActionResult> DeleteStudio(int adminId)
        {
            await _infoAdminStudioService.DeleteStudio(adminId);
            return RedirectToAction("Index", "AdminStudio", new { adminId = adminId });  //перед '=' ДОЛЖНО совпадать с параметром в AdminStudioController Index
        }


        /// <summary>
        /// Изменение данных администратора
        /// </summary>
        /// <param name="updatedAdmin"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateAdmin(AdminStudioDetailsViewModel viewModel)
        {
            var admin = await _adminStudioService.FindAdmin(viewModel.Admin.IdAdmin);
            if (admin == null)
            {
                return NotFound();
            }

            admin.Name = viewModel.Admin.Name;
            admin.Surname = viewModel.Admin.Surname;
            admin.Email = viewModel.Admin.Email;

            await _adminStudioService.UpdateAdmin(admin);

            return RedirectToAction("Index", new { adminId = admin.IdAdmin });
        }

        /// <summary>
        /// Удаление студии
        /// </summary>
        /// <param name="adminId"></param>
        /// <returns></returns>
        public async Task<IActionResult> DeleteAdmin(int adminId)
        {
            await _infoAdminStudioService.DeleteAdmin(adminId);
            return RedirectToAction("Index", "Home");  //перед '=' ДОЛЖНО совпадать с параметром в AdminStudioController Index
        }
    }
}
