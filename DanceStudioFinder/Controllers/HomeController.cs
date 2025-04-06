using DanceStudioFinder.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Diagnostics;

namespace DanceStudioFinder.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _connectionString;
        public HomeController(IConfiguration configuration) 
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        /*public IActionResult Index()
        {
            return View();
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
        public IActionResult Index()
        {
            var danceStudios = new List<DanceStudio>();
            try
            {
                using(var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand("select * from dance_studio", connection))
                    using(var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var studio = new DanceStudio
                            {
                                IdStudio = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                IdAddress = reader.GetInt32(2),
                                PhoneNumber = reader.GetString(3),
                                ExtraPhoneNumber = reader.IsDBNull(4) ? null : reader.GetString(4),
                                VkGroup = reader.IsDBNull(5) ? null : reader.GetString(5),
                                Website = reader.IsDBNull(6) ? null : reader.GetString(6),
                                Telegram = reader.IsDBNull(7) ? null : reader.GetString(7),
                                IdAdmin = reader.GetInt32(8),
                            };
                            danceStudios.Add(studio);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            var viewModel = new UserViewModel
            {
                // Присваиваем список DanceStudio свойству DanceStudios
                DanceStudios = danceStudios
            };

            return View(viewModel);
            //return View(danceStudios);
        }
    }
}
