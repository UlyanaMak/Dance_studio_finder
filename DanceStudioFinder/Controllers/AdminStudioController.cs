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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">чтобы был корректный адрес</param>
        /// <returns></returns>
        public async Task<IActionResult> Index(int adminId)
        {
            // Ожидаем результат асинхронной операции
            var admin = await _adminStudioService.FindAdmin(adminId);

            if (admin == null)
            {
                return NotFound("Администратор не найден.");
            }
            var adminStudio = await _adminStudioService.FindStudio(adminId);
            if (adminStudio == null)
            {
                var viewModel = new AdminStudioViewModel
                {
                    Admin = admin,
                    DanceStudio = null //студии нет
                };
                return View("CreateStudio", viewModel);
            }
            var studioViewModel = new AdminStudioViewModel
            {
                Admin = admin,
                DanceStudio = adminStudio,
            };
            //передаем в представление объект admin
            return RedirectToAction("Studio", studioViewModel);
        }

        
        public async Task<IActionResult> CreateStudio(int adminId)
        {
            var admin = await _adminStudioService.FindAdmin(adminId);
            if (admin == null)
            {
                return NotFound();
            }

            // Создаем модель для представления
            var viewModel = new AdminStudioViewModel
            {
                Admin = admin,
                DanceStudio = null // Так как это форма создания
            };

            return View(viewModel);
        }
    }
}



