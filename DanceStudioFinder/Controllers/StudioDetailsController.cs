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
        public IActionResult Index(int id)
        {
            var studio = _context.DanceStudios.FirstOrDefault(s => s.IdStudio == id);

            if (studio == null)
            {
                return NotFound(); 
            }

            return View(studio);
        }
    }
}
