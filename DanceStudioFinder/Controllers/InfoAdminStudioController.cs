using DanceStudioFinder.Models;
using DanceStudioFinder.Services;
using Microsoft.AspNetCore.Mvc;

namespace DanceStudioFinder.Controllers
{
    public class InfoAdminStudioController : Controller
    {
        private readonly InfoAdminStudioService _infoAdminStudioService;
        private readonly AdminStudioService _adminStudioService;
        public InfoAdminStudioController(InfoAdminStudioService infoAdminStudioService, AdminStudioService adminStudioService)
        {
            _infoAdminStudioService = infoAdminStudioService;
            _adminStudioService = adminStudioService;
        }
        public async Task<IActionResult> Index(int adminId)
        {
            var admin = await _adminStudioService.FindAdmin(adminId);  //нахождение администратора по id

            if (admin == null)  //если не найден
            {
                return NotFound("Администратор не найден.");  //сообщение об ошибке
            }

            return View(admin);
        }

        public async Task<IActionResult> DeleteStudio(int adminId)
        {
            await _infoAdminStudioService.DeleteStudio(adminId);
            return RedirectToAction("Index", "Home");
        }
    }
}
