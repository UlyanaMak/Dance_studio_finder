using DanceStudioFinder.Data;
using DanceStudioFinder.Models;
using DanceStudioFinder.Services;
using DanceStudioFinder.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DanceStudioFinder.Controllers
{
    public class InfoAdminStudioController : Controller
    {
        private readonly InfoAdminStudioService _infoAdminStudioService;
        private readonly AdminStudioService _adminStudioService;
        private readonly ApplicationDbContext _context;
        private readonly OpenStreetMapService _openStreetMapsService;
        public InfoAdminStudioController(InfoAdminStudioService infoAdminStudioService, 
            AdminStudioService adminStudioService, 
            ApplicationDbContext context,
            OpenStreetMapService openStreetMapService)
        {
            _infoAdminStudioService = infoAdminStudioService;
            _adminStudioService = adminStudioService;
            _context = context;
            _openStreetMapsService = openStreetMapService;
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

        public async Task<IActionResult> UpdateStudio(int  adminId)
        {
            var admin = await _adminStudioService.FindAdmin(adminId);  //нахождение администратора по id
            if (admin == null)  //если не существует (это невозможно, но на всякий случай)
            {
                return NotFound();  //ошибка
            }

            return RedirectToAction("UpdateAddressStudio", new { adminId = adminId });
        }

        /// <summary>
        /// (1) страница редактирования студии
        /// </summary>
        /// <param name="adminId"></param>
        /// <returns></returns>
        public async Task<IActionResult> UpdateAddressStudio(int adminId)
        {
            var admin = await _adminStudioService.FindAdmin(adminId);  //нахождение администратора по id
            if (admin == null)  //если не существует (это невозможно, но на всякий случай)
            {
                return NotFound();  //ошибка
            }
            var studio = await _adminStudioService.FindStudio(adminId);  //нахождение студии
            if (studio == null)  //если не существует (это невозможно, но на всякий случай)
            {
                return NotFound();  //ошибка
            }
            var address = await _infoAdminStudioService.FindAddress(studio.IdAddress);
            if (address == null)
            {
                return NotFound();
            }
            //модель для представления
            var viewModel = new CreateAddressStudioViewModel
            {
                Admin = admin,                                 //текущий администратор
                DanceStudio = studio,                          //данные студии
                Address = address                              //адрес студии
            };
            return View(viewModel);  //передача модели  представление
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAddressStudio(CreateAddressStudioViewModel viewModel)
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
                await _infoAdminStudioService.UpdateAddress(viewModel.Address); //обновление адреса
            }
            else //если существует
            {
                addressIndex = addressExist;
            }

            viewModel.DanceStudio.IdAddress = viewModel.Address.IdAddress;
            viewModel.DanceStudio.IdAdmin = admin.IdAdmin;

            ModelState.Remove("DanceStudio.IdAddressNavigation");
            ModelState.Remove("DanceStudio.IdAdminNavigation");

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            await _infoAdminStudioService.UpdateStudio(viewModel.DanceStudio); //обновление данных студии


            // После успешного сохранения — можно перенаправить
            return RedirectToAction("UpdatePricesStudio", new { adminId = admin.IdAdmin });
        }

        /// <summary>
        /// (2) страница изменения цен студии
        /// </summary>
        /// <param name="adminId"></param>
        /// <returns></returns>
        public async Task<IActionResult> UpdatePricesStudio(int adminId)
        {
            var admin = await _adminStudioService.FindAdmin(adminId);  //нахождение администратора по id
            if (admin == null)  //если не существует (это невозможно, но на всякий случай)
            {
                return NotFound();  //ошибка
            }
            var adminStudio = await _infoAdminStudioService.FindStudioWithPrices(adminId);
            //модель для представления
            var viewModel = new CreatePricesStudioViewModel
            {
                Admin = admin,                                 //текущий администратор
                DanceStudio = adminStudio,                     //студия
                Prices = adminStudio.Prices.ToList(),
            };
            return View(viewModel);  //передача модели  представление
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePricesStudio(CreatePricesStudioViewModel viewModel)
        {
            var pricesData = Request.Form["PricesData"];
            if (!string.IsNullOrEmpty(pricesData))
            {
                viewModel.Prices = JsonConvert.DeserializeObject<List<Price>>(pricesData);
            }

            if (viewModel.Prices == null || viewModel.Prices.Count == 0)
            {
                ModelState.AddModelError("", "Необходимо добавить хотя бы одну цену");
                return View(viewModel);
            }

            try
            {
                // 1. Удаляем все старые цены для этой студии
                var deleteResult = await _infoAdminStudioService.DeleteAllPricesForStudio(viewModel.DanceStudio.IdStudio);
                if (!deleteResult)
                {
                    ModelState.AddModelError("", "Ошибка при удалении старых цен");
                    return View(viewModel);
                }

                // 2. Сохраняем новые цены
                foreach (var price in viewModel.Prices)
                {
                    var saveResult = await _adminStudioService.SavePrice(viewModel.DanceStudio.IdStudio, price);
                    if (!saveResult)
                    {
                        ModelState.AddModelError("price.Price1", "Произошла ошибка при сохранении цены");
                        return View(viewModel);
                    }
                }

                return RedirectToAction("UpdateScheduleStudio", new { adminId = viewModel.Admin.IdAdmin });  //перенаправление на страницу изменения расписания
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Произошла ошибка при обновлении цен: {ex.Message}");
                return View(viewModel);
            }
        }

        /// <summary>
        /// (3) страница изменения групп и их расписания в студии
        /// </summary>
        /// <param name="adminId"></param>
        /// <returns></returns>
        public async Task<IActionResult> UpdateScheduleStudio(int adminId)
        {
            var admin = await _adminStudioService.FindAdmin(adminId);
            if (admin == null) return NotFound();

            var studio = await _adminStudioService.FindStudio(adminId);
            var styles = _adminStudioService.GetStyles();
            var days = _adminStudioService.GetWeekDays();

            // Получаем существующие группы с расписаниями
            var existingGroups = await _infoAdminStudioService.GetGroupsWithSchedules(studio.IdStudio);

            var viewModel = new CreateScheduleStudioViewModel
            {
                Admin = admin,
                DanceStudio = studio,
                Styles = styles,
                WeekDays = days,
                Groups = existingGroups.Select(g => new GroupViewModel
                {
                    Name = g.Name,
                    StyleId = g.StyleId,
                    MinAge = g.MinAge,
                    MaxAge = g.MaxAge,
                    Description = g.Description,
                    Schedule = g.Schedules.Select(s => new ScheduleViewModel
                    {
                        DayOfWeekId = s.IdDay,
                        BeginTime = s.BeginTime,
                        EndTime = s.EndTime
                    }).ToList()
                }).ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateScheduleStudio(CreateScheduleStudioViewModel viewModel, string groupsData)
        {
            int idAdmin = viewModel.Admin.IdAdmin;
            int idStudio = viewModel.DanceStudio.IdStudio;
            try
            {
                var settings = new JsonSerializerSettings
                {
                    Converters = new List<JsonConverter> { new TimeOnlyJsonConverter() }
                };

                var groups = JsonConvert.DeserializeObject<List<GroupViewModel>>(groupsData, settings);

                //текущие группы студии
                var existingGroups = await _infoAdminStudioService.GetGroupsByStudioId(idStudio);

                //удаление всех существующие группы и их расписание
                foreach (var existingGroup in existingGroups)
                {
                    await _infoAdminStudioService.DeleteSchedulesByGroupId(existingGroup.IdGroup);
                    await _infoAdminStudioService.DeleteGroup(existingGroup.IdGroup);

                    //используется ли AgeLimit другими студиями
                    var isAgeLimitUsed = await _infoAdminStudioService.IsAgeLimitUsedByOtherGroups(
                        existingGroup.IdAgeLimit, existingGroup.IdGroup);

                    if (!isAgeLimitUsed)
                    {
                        await _infoAdminStudioService.DeleteAgeLimit(existingGroup.IdAgeLimit);
                    }
                }

                //новые группы и расписание
                foreach (var groupVm in groups)
                {
                    //проверка/создание AgeLimit
                    var ageLimit = _adminStudioService.FindAgeLimits(groupVm.MinAge, groupVm.MaxAge);
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

                    //группа
                    var group = new DanceGroup
                    {
                        Name = groupVm.Name,
                        IdStyle = groupVm.StyleId,
                        IdStudio = idStudio,
                        IdAgeLimit = ageLimit.IdAgeLimit,
                        Description = groupVm.Description,
                    };

                    await _adminStudioService.SaveGroup(group);

                    //расписание
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

                return RedirectToAction("Index", "InfoAdminStudio", new { adminId = idAdmin });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Произошла ошибка при обновлении расписания: {ex.Message}");
                return View(viewModel);
            }
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
