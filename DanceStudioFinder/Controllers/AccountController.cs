using DanceStudioFinder.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DanceStudioFinder.DataBase;

namespace DanceStudioFinder.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context; 

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Users.AnyAsync(u => u.Email == model.Register.RegisterEmail))
                {
                    ModelState.AddModelError("Register.RegisterEmail", "Такой пользователь уже зарегистрирован.");
                    return View("Index", model); // Возвращаем в то же представление с моделью, чтобы отобразить ошибку
                }
                var user = new User
                {
                    Name = model.Register.RegisterName,
                    Surname = model.Register.RegisterSurname,
                    Email = model.Register.RegisterEmail,
                    Password = Models.User.GetPasswordHash(model.Register.RegisterPassword)
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View("Index", model);
        }

        public IActionResult Login()
        {
            return View();
        }
    }
}
