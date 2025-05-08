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


        /// <summary>
        /// Удаление студии
        /// </summary>
        /// <param name="adminId"></param>
        /// <returns></returns>
        public async Task<IActionResult> DeleteStudio(int adminId)
        {
            await _infoAdminStudioService.DeleteStudio(adminId);
            return RedirectToAction("Index", "Home");
        }


        /// <summary>
        /// Изменение данных администратора
        /// </summary>
        /// <param name="updatedAdmin"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateAdmin(Admin updatedAdmin)
        {
            var admin = await _adminStudioService.FindAdmin(updatedAdmin.IdAdmin);
            if (admin == null)
            {
                return NotFound();
            }

            admin.Name = updatedAdmin.Name;
            admin.Surname = updatedAdmin.Surname;
            admin.Email = updatedAdmin.Email;

            await _adminStudioService.UpdateAdmin(admin);

            return RedirectToAction("Index", new { adminId = admin.IdAdmin });
        }
    }
}
