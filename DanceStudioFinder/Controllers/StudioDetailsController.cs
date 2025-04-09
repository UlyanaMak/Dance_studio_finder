using DanceStudioFinder.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DanceStudioFinder.Controllers
{
    public class StudioDetailsController : Controller
    {
        private readonly ApplicationDbContext _context; // Замените ApplicationDbContext на ваш DbContext

        public StudioDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int id)
        {
            var studio = _context.DanceStudios.FirstOrDefault(s => s.IdStudio == id);

            // 2. Проверяем, что студия найдена
            if (studio == null)
            {
                return NotFound(); // Или обработайте ситуацию, когда студия не найдена (например, перенаправление на другую страницу)
            }

            // 3. Передаем данные о студии в View
            return View(studio); // Передаем объект студии в представление
        }
    }
}
