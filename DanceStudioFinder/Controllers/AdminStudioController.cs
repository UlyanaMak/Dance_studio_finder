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
        /// Начальная страница (выбор того, что открыть: создание или информация)
        /// </summary>
        /// <param name="id">чтобы был корректный адрес</param>
        /// <returns></returns>
        public async Task<IActionResult> Index(int adminId)
        {
            var admin = await _adminStudioService.FindAdmin(adminId);  //нахождение администратора по id

            if (admin == null)  //если не найден
            {
                return NotFound("Администратор не найден.");  //сообщение об ошибке
            }
            var adminStudio = await _adminStudioService.FindStudio(adminId);  //поиск студии администратора
            if (adminStudio == null)  //студии нет
            {
                /*var viewModel = new AdminStudioViewModel
                {
                    Admin = admin,
                    DanceStudio = null //студии нет
                };*/
                return RedirectToAction("CreateStudio", new { adminId = adminId });     //переход на страницу с созданием студии (1)
                //return View("CreateStudio", viewModel);
            }
            //если студия есть
            var studioViewModel = new AdminStudioViewModel  //создаем модель для представления информации о студии
            {
                Admin = admin,
                DanceStudio = adminStudio,
            }; 
            return RedirectToAction("Studio", studioViewModel);  //преедача модели в представление с информацией о студии
        }

        
        /// <summary>
        /// (1) страница создания студии
        /// </summary>
        /// <param name="adminId"></param>
        /// <returns></returns>
        public async Task<IActionResult> CreateStudio(int adminId)
        {
            var admin = await _adminStudioService.FindAdmin(adminId);  //нахождение администратора по id
            if (admin == null)  //если не существует (это невозможно, но на всякий случай)
            {
                return NotFound();  //ошибка
            }
            //модель для представления
            var viewModel = new AdminStudioViewModel
            {
                Admin = admin,  //текущий администратор
                DanceStudio = null //студии пока нет
            };

            return View(viewModel);  //передача модели  представление
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStudio(AdminStudioViewModel viewModel)
        {
            var admin = await _adminStudioService.FindAdmin(viewModel.Admin.IdAdmin);
            viewModel.Admin = admin;

            if (!ModelState.IsValid)
            {
                // Если валидация не прошла — вернем форму с ошибками
                return View(viewModel);
            }

            // Логика сохранения студии
            //await _adminStudioService.CreateStudio(viewModel);

            // После успешного сохранения — можно перенаправить
            return RedirectToAction("Studio", new { adminId = viewModel.Admin.IdAdmin });
        }
    }
}



