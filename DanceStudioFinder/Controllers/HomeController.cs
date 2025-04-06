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
                .Include(ds => ds.IdAddressNavigation) 
                .ToList();

            var viewModel = new UserViewModel
            {
                DanceStudios = danceStudios
            };

            return View(viewModel);
        }
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
