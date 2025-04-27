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
            return PartialView("_LoginModal");
        }

        [HttpGet]
        public ActionResult Register()
        {
            return PartialView("_RegisterModal", new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserViewModel userModel)
        {
            //�������� �������� ������� ��������
            ModelState.Remove("Login");
            ModelState.Remove("DanceStudios");

            if (ModelState.IsValid)  //�������� ������ �� ����������
            {
                try
                {
                    Admin admin = new Admin  //�������� �������������� ��� ������
                    {
                        Name = userModel.Register.RegisterName,
                        Surname = userModel.Register.RegisterSurname,
                        Email = userModel.Register.RegisterEmail
                    };

                    var result = await _adminService.RegisterAdmin(admin, userModel.Register.RegisterPassword);  //����������� �������������� (������ ���������� � AdminService)

                    if (result)  //���� �������
                    {
                        return RedirectToAction("Index", "AddStudio");  //��������������� �� �������� ���������� ������
                    }
                    else  // ���� ������������ � ����� ������ ����������
                    {
                        ModelState.AddModelError("Register.RegisterEmail", "������������ � ������ ��. ������ ��� ���������������");  // �������� �� ���� ������������
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "��������� ������ ��� �����������. ���������� � ��������������.");
                }
            }

            // ���� ���� ������ (��������� ��� email ��� ����������), ���������� Index View
            // � �������� userModel, ����� ���������� ��������� ���� � ������
            return PartialView("_RegisterModal", userModel.Register);
        }

        /*[HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _adminService.ValidateAdmin(model))
                {
                    var admin = await _adminService.GetAdminByEmail(model.LoginEmail);

                    // ������� Claims (�����������) ��������������
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, admin.Name),
                    
                    new Claim(ClaimTypes.Email, admin.Email)
                        // �������� ������ claims, ���� �����
                };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme); Scheme, claimsPrincipal, authProperties);

                    // �������� ����, �������������� �� ������� ��������
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "�������� ����� ��� ������.");
                    return PartialView("_LoginModal", model);  // ���������� ��������� ���� � ��������
                }
            }

            // ���� ������ �� �������, ���������� ��������� ������������� � ��������
            return PartialView("_LoginModal", model);
        }*/
    }
}
