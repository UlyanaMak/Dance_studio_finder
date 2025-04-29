using DanceStudioFinder.Models;
using DanceStudioFinder.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DanceStudioFinder.Controllers
{
    public class AdminStudioController : Controller
    {
        private readonly AdminStudioService _adminStudioService;
        public AdminStudioController(AdminStudioService adminStudioService)
        {
            _adminStudioService = adminStudioService;
        }
        public async Task<IActionResult> Index(int adminId)
        {
            // Ожидаем результат асинхронной операции
            var admin = await _adminStudioService.FindAdmin(adminId);

            if (admin == null)
            {
                return NotFound("Администратор не найден.");
            }

            //передаем в представление объект admin
            return View(admin);
        }
    }
}


