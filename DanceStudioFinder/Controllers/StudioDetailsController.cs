using DanceStudioFinder.Data;
using DanceStudioFinder.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DanceStudioFinder.Controllers
{
    public class StudioDetailsController : Controller
    {
        private readonly ApplicationDbContext _context; 

        public StudioDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Вывод страницы с информацией о студии
        /// </summary>
        /// <param name="studioId"></param>
        /// <returns></returns>
        public async Task<IActionResult> Index(int studioId)
        {
            var studio = await _context.DanceStudios
                .Include(s => s.IdAddressNavigation)
                .Include(s => s.Prices)
                .Include(s => s.DanceGroups)
                    .ThenInclude(g => g.IdStyleNavigation)
                .Include(s => s.DanceGroups)
                    .ThenInclude(g => g.IdAgeLimitNavigation)
                .Include(s => s.DanceGroups)
                    .ThenInclude(g => g.Schedules)
                        .ThenInclude(sch => sch.IdDayNavigation)
                .FirstOrDefaultAsync(s => s.IdStudio == studioId);

            if (studio == null)
            {
                return NotFound();
            }

            var viewModel = new StudioDetailsViewModel
            {
                Studio = studio,
                Groups = studio.DanceGroups.Select(g => new DanceGroupViewModel
                {
                    IdGroup = g.IdGroup,
                    Name = g.Name,
                    Description = g.Description,
                    Style = g.IdStyleNavigation,
                    AgeLimit = g.IdAgeLimitNavigation,
                    Schedule = g.Schedules.Select(s => new ScheduleDisplayModel
                    {
                        Day = s.IdDayNavigation,
                        BeginTime = s.BeginTime,
                        EndTime = s.EndTime
                    }).ToList()
                }).ToList(),
                Prices = studio.Prices.ToList()
            };

            return View(viewModel);
        }
    }
}
