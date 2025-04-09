using DanceStudioFinder.Data;
using DanceStudioFinder.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Diagnostics;

namespace DanceStudioFinder.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context; 

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var danceStudios = _context.DanceStudios
                .Include(ds => ds.IdAddressNavigation) // загрузка адресов
                .Include(ds => ds.Prices)              // загрузка цен
                .ToList();

            var viewModel = new UserViewModel  //созадние модели для главной страницы
            {
                DanceStudios = danceStudios
            };

            return View(viewModel);
        }

        /*public IActionResult StudioDetails(int id)
        {
            var studio = _context.DanceStudios.FirstOrDefault(s => s.IdStudio == id);

            *//*//проверка, что студия найдена
            if (studio == null)
            {
                return NotFound(); // Или обработайте ситуацию, когда студия не найдена (например, перенаправление на другую страницу)
            }*//*

            return View(studio); // передача объекта студии в View
        }*/

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(UserViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                ViewData["ModelIsValid"] = true;
                return View();
            }
            ViewData["ModelIsValid"] = false;
            return View();
        }
    }
}
