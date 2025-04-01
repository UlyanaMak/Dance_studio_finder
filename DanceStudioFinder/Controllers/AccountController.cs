using Microsoft.AspNetCore.Mvc;

namespace DanceStudioFinder.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
