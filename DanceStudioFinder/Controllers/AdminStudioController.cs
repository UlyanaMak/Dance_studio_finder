using DanceStudioFinder.Models;
using DanceStudioFinder.ViewModels;
using DanceStudioFinder.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace DanceStudioFinder.Controllers
{
    public class AdminStudioController : Controller
    {
        private readonly AdminStudioService _adminStudioService;
        private readonly OpenStreetMapService _openStreetMapsService;
        public AdminStudioController(AdminStudioService adminStudioService, OpenStreetMapService openStreetMapsService)
        {
            _adminStudioService = adminStudioService;
            _openStreetMapsService = openStreetMapsService;
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

            //!!!!!!!!!!!!!!!!Здесь измнеить модель представления
            //если студия есть
            var studioViewModel = new CreateAddressStudioViewModel  //создаем модель для представления информации о студии
            {
                Admin = admin,
                DanceStudio = adminStudio,
            };
            return RedirectToAction("Studio", studioViewModel);  //преедача модели в представление с информацией о студии
            //!!!!!!!!!!!!!!!!Здесь измнеить модель представления
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
            var (isValid, settlementArea) = await _openStreetMapsService.ValidateAddressAsync(
                    viewModel.Address.Entity,
                    viewModel.Address.Locality,
                    viewModel.Address.Street,
                    viewModel.Address.BuildingNumber,
                    viewModel.Address.Letter);

            if (!isValid)  //если адрес не найден
            {
                ModelState.AddModelError("Address.Entity", "Адрес не найден. Пожалуйста, проверьте правильность введённых данных.");  //сообщение
                return View(viewModel);  //возврат на текущую страницу с сообщением
            }

            //установка района (м.б. null)
            viewModel.Address.SettlementArea = settlementArea;

            int addressIndex;
            int addressExist = await _adminStudioService.FindAddress(viewModel.Address); //проверка, существует ли такой адрес
            if (addressExist == 0)  //если не существует
            {
                //создаём адрес
                addressIndex = await _adminStudioService.SaveAddress(viewModel.Address); //извлекаем индекс
            }
            else //если существует
            {
                addressIndex = addressExist;
            }

            viewModel.DanceStudio.IdAddress = addressIndex;
            viewModel.DanceStudio.IdAdmin = admin.IdAdmin;
            ModelState.Remove("DanceStudio.IdAddressNavigation");
            ModelState.Remove("DanceStudio.IdAdminNavigation");

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var result = await _adminStudioService.SaveStudio(addressIndex, admin, viewModel.DanceStudio);

            if (!result)
            {
                ModelState.AddModelError("", "Произошла ошибка при сохранении студии");
                return View(viewModel);
            }

            // После успешного сохранения — можно перенаправить
            return RedirectToAction("CreatePricesStudio", new { adminId = admin.IdAdmin });
        }


        /// <summary>
        /// (2) страница создания цен студии
        /// </summary>
        /// <param name="adminId"></param>
        /// <returns></returns>
        public async Task<IActionResult> CreatePricesStudio(int adminId)
        {
            var admin = await _adminStudioService.FindAdmin(adminId);  //нахождение администратора по id
            if (admin == null)  //если не существует (это невозможно, но на всякий случай)
            {
                return NotFound();  //ошибка
            }
            var adminStudio = await _adminStudioService.FindStudio(adminId);
            //модель для представления
            var viewModel = new CreatePricesStudioViewModel
            {
                Admin = admin,                                 //текущий администратор
                DanceStudio = adminStudio,                     //студия
            };
            return View(viewModel);  //передача модели  представление
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePricesStudio(CreatePricesStudioViewModel viewModel, string PricesData)
        {
            if (!string.IsNullOrEmpty(PricesData))
            {
                var prices = JsonConvert.DeserializeObject<List<Price>>(PricesData);
                viewModel.Prices = prices;
            }

            if (viewModel.Prices == null || viewModel.Prices.Count == 0)
            {
                ModelState.AddModelError("", "Необходимо добавить хотя бы одну цену");
                return View(viewModel);
            }

            try
            {
                foreach (var price in viewModel.Prices)
                {
                    var result = await _adminStudioService.SavePrice(viewModel.DanceStudio.IdStudio, price);
                    if (!result)
                    {
                        ModelState.AddModelError("price.Price1", "Произошла ошибка при сохранении студии");
                        return View(viewModel);
                    }
                }
                return RedirectToAction("CreateScheduleStudio", new { adminId = viewModel.Admin.IdAdmin });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Произошла ошибка при сохранении цен");
                return View(viewModel);
            }
        }


        /// <summary>
        /// (3) страница создания групп и их расписания в студии
        /// </summary>
        /// <param name="adminId"></param>
        /// <returns></returns>
        public async Task<IActionResult> CreateScheduleStudio(int adminId)
        {
            var admin = await _adminStudioService.FindAdmin(adminId);  //нахождение администратора по id
            if (admin == null) return NotFound();

            var studio = await _adminStudioService.FindStudio(adminId);
            var styles = _adminStudioService.GetStyles(); // Получаем стили
            var days = _adminStudioService.GetWeekDays();  // Дни недели
            //модель для представления
            var viewModel = new CreateScheduleStudioViewModel
            {
                Admin = admin,
                DanceStudio = studio,
                Styles = styles,
                WeekDays = days
            };
            return View(viewModel);  //передача модели  представление


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateScheduleStudio(CreateScheduleStudioViewModel viewModel)
        {
            var adminStudio = await _adminStudioService.FindStudio(viewModel.Admin.IdAdmin);
            if (adminStudio == null) return NotFound();
            viewModel.DanceStudio = adminStudio;
            /*ModelState.Remove("DanceStudio");
            if (!ModelState.IsValid)
            {
                //повторная загрузка стилей и дней недели если форма невалидна
                viewModel.Styles = _adminStudioService.GetStyles();
                viewModel.WeekDays = _adminStudioService.GetWeekDays();
                return View(viewModel);
            }*/
            foreach (var groupVm in viewModel.Groups) //перебор всех существующих групп
            {
                var ageLimit = _adminStudioService.FindAgeLimits(groupVm.MinAge, groupVm.MaxAge);  //проверка, существует ли данное возрастное ограничение
                if (ageLimit == null)
                {
                    ageLimit = new AgeLimit
                    {
                        MinAge = groupVm.MinAge,
                        MaxAge = groupVm.MaxAge,
                        Name = GenerateAgeLimitName(groupVm.MinAge, groupVm.MaxAge)
                    };
                    await _adminStudioService.SaveAgeLimit(ageLimit);
                }

                var group = new DanceGroup
                {
                    Name = groupVm.Name,
                    IdStyle = groupVm.StyleId,
                    IdStudio = viewModel.DanceStudio.IdStudio,
                    IdAgeLimit = ageLimit.IdAgeLimit,
                    Description = groupVm.Description,
                };

                await _adminStudioService.SaveGroup(group);

                foreach (var scheduleVm in groupVm.Schedule)
                {
                    var schedule = new Schedule
                    {
                        IdGroup = group.IdGroup,
                        IdDay = scheduleVm.DayOfWeekId,
                        BeginTime = scheduleVm.BeginTime,
                        EndTime = scheduleVm.EndTime
                    };
                    await _adminStudioService.SaveSchedule(schedule);
                }
            }
            TempData["Success"] = "Группы и расписание успешно добавлены.";
            return RedirectToAction("Index", "AdminStudio");
        }

        private string GenerateAgeLimitName(int? min, int? max)
        {
            if (min == null && max == null)
                return "Для всех возрастов";
            if (min != null && max == null)
                return $"{min}+";
            if (min != null && max != null)
                return $"{min}-{max}";
            return $"До {max}";
        }
    }
}



