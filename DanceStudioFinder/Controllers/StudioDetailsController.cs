using DanceStudioFinder.Data;
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
        public IActionResult Index(int studioId)
        {
            var studio = _context.DanceStudios.FirstOrDefault(s => s.IdStudio == studioId);  //извлечение студии по id

            if (studio == null)
            {
                return NotFound(); 
            }

            return View(studio);  //возврат представления с объектом
        }
    }
}
