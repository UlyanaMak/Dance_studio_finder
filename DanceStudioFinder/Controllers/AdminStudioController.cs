using DanceStudioFinder.Models;
using DanceStudioFinder.ViewModels;
using DanceStudioFinder.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DanceStudioFinder.Controllers
{
    public class AdminStudioController : Controller
    {
        private readonly AdminStudioService _adminStudioService;
        private readonly YandexMapsService _yandexMapsService;
        public AdminStudioController(AdminStudioService adminStudioService, YandexMapsService yandexMapsService)
        {
            _adminStudioService = adminStudioService;
            _yandexMapsService = yandexMapsService; 
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
                return RedirectToAction("CreateAddressStudio", new { adminId = adminId });     //переход на страницу с созданием студии (1)
            }
            //если студия есть
            var studioViewModel = new CreateAddressStudioViewModel  //создаем модель для представления информации о студии
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
        public async Task<IActionResult> CreateAddressStudio(int adminId)
        {
            var admin = await _adminStudioService.FindAdmin(adminId);  //нахождение администратора по id
            if (admin == null)  //если не существует (это невозможно, но на всякий случай)
            {
                return NotFound();  //ошибка
            }
            //модель для представления
            var viewModel = new CreateAddressStudioViewModel
            {
                Admin = admin,                                 //текущий администратор
                DanceStudio = null,                            //студии пока нет
            };
            return View(viewModel);  //передача модели  представление
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAddressStudio(CreateAddressStudioViewModel viewModel)
        {
            var admin = await _adminStudioService.FindAdmin(viewModel.Admin.IdAdmin);
            viewModel.Admin = admin;
            var (isValid, settlementArea) = await _yandexMapsService.ValidateAddressAsync(
                    viewModel.Address.Entity,
                    viewModel.Address.Locality,
                    viewModel.Address.Street,
                    viewModel.Address.BuildingNumber,
                    viewModel.Address.Letter);

            if (!isValid)
            {
                ModelState.AddModelError("Address", "Адрес не найден на карте. Пожалуйста, проверьте правильность введённых данных.");
                return View(viewModel);
            }

            // Устанавливаем район, если он найден
            viewModel.Address.SettlementArea = settlementArea;

            if (!ModelState.IsValid)  //ОСТАНОВИЛАСЬ ТУТ (НЕВАЛИДНЫЕ СВЯЗИ С ADMIN И ADDRESS
            {
                // Если валидация не прошла — вернем форму с ошибками
                return View(viewModel);
            }
            
            // Логика сохранения студии
            //await _adminStudioService.CreateAddressStudio(viewModel);

            // После успешного сохранения — можно перенаправить
            return RedirectToAction("Studio", new { adminId = viewModel.Admin.IdAdmin });
        }
    }
}



