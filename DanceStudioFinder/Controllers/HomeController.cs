using DanceStudioFinder.Data;
using DanceStudioFinder.Models;
using DanceStudioFinder.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Diagnostics;
using System.Security.Claims;

namespace DanceStudioFinder.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAdminService _adminService;
        public HomeController(ApplicationDbContext context, IAdminService adminService)
        {
            _context = context;
            _adminService = adminService;
        }

        public IActionResult Index()
        {
            var danceStudios = _context.DanceStudios
                .Include(ds => ds.IdAddressNavigation) // �������� �������
                .Include(ds => ds.Prices)              // �������� ���
                .ToList();

            var viewModel = new UserViewModel  //�������� ������ ��� ������� ��������
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

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }


        /// <summary>
        /// ����������� ��������������
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserViewModel model)
        {
            ModelState.Remove("Login");  //������� �������� �����
            ModelState.Remove("DanceStudios");  //������� �������� ������������ ������
            
            if (!ModelState.IsValid)  //���� �� �������� ���������
            {
                model.DanceStudios = _context.DanceStudios.ToList();  //������ ������
                return View("Index", model);  //�� ������� �������� � ��������� �����
            }
            //���� �������� ���������
            try
            {
                Admin admin = new Admin  //�������� �������������� ��� ������
                {
                    Name = model.Register.RegisterName,
                    Surname = model.Register.RegisterSurname,
                    Email = model.Register.RegisterEmail
                };

                var result = await _adminService.RegisterAdmin(admin, model.Register.RegisterPassword);  //����������� �������������� (������ ���������� � AdminService)

                if (result)  //���� �������
                {
                    return RedirectToAction("Index", "AdminStudio", new { adminId = admin.IdAdmin });  //��������������� �� �������� ������, ����������� � ��������������
                }
                else  //���� ������������� � ����� ������ ����������
                {
                    ModelState.AddModelError("Register.RegisterEmail", "������������ � ������ ��. ������ ��� ���������������. ������� � �������.");  //���������
                }
            }
            catch (Exception ex)  //���� ��������� ������
            {
                ModelState.AddModelError(string.Empty, "��������� ������ ��� �����������. ���������� � ��������� �����.");
            }
            model.DanceStudios = _context.DanceStudios.ToList();  //������ ���� ������ � �������
            return View("Index", model);  //������� �� �������
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserViewModel model)
        {
            ModelState.Remove("Register");  //������� �������� �����������
            ModelState.Remove("DanceStudios");  //������� �������� ������������ ������

            if (!ModelState.IsValid)
            {
                model.DanceStudios = _context.DanceStudios.ToList();  //������ ������
                return View("Index", model);
            }
            var result = await _adminService.ValidateAdmin(model.Login);
            var admin = await _adminService.GetAdminByEmail(model.Login.LoginEmail);

            if (admin == null)
            {
                ModelState.AddModelError("Login.LoginEmail", "������������ � ������ ��. ������ ��� �� ���������������.");
            }
            else if (result)
            {
                return RedirectToAction("Index", "AdminStudio", new { adminId = admin.IdAdmin });  //����� '=' ������ ��������� � ���������� � AdminStudioController Index
            }
            else
            {
                ModelState.AddModelError("Login.LoginPassword", "������ �����������.");
            }

            // ���� ������ �� �������, ���������� ��������� ������������� � ��������
            model.DanceStudios = _context.DanceStudios.ToList();  //������ ���� ������ � �������
            return View("Index", model);  //������� �� �������
        }
    }
}
