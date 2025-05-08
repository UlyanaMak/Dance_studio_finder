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

            var styles = _context.Styles.OrderBy(s => s.IdStyle).ToList();  //�����

            var studioFilter = new StudioFilterModel  //�������������
            {
                Styles = styles
            };

            var viewModel = new UserViewModel  //�������� ������ ��� ������� ��������
            {
                DanceStudios = danceStudios,  //������ ������
                StudioFilter = studioFilter   //���������� ������
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(StudioFilterModel filter)
        {
            //������� ������ ���� ������ ��
            var danceStudios = _context.DanceStudios
                .Include(ds => ds.IdAddressNavigation)
                .Include(ds => ds.Prices)
                .Include(ds => ds.DanceGroups)
                    .ThenInclude(group => group.Schedules)
                .Include(ds => ds.DanceGroups)
                    .ThenInclude(group => group.IdStyleNavigation)
                .AsQueryable();

            //����������� ��������
            danceStudios = ApplyFilters(danceStudios, filter);
            //��������������� ������ ��� �������������
            var filteredViewModel = new UserViewModel
            {
                DanceStudios = danceStudios.ToList(),
                StudioFilter = filter
            };

            if (!filteredViewModel.DanceStudios.Any())
            {
                ViewBag.NoResultsMessage = "��� ������, ���������� ��� �������� �������.";
            }

            return View(filteredViewModel);
        }

        /// <summary>
        /// ���������� ��������
        /// </summary>
        /// <param name="query"></param>
        /// <param name="filter">�������</param>
        /// <returns></returns>
        private IQueryable<DanceStudio> ApplyFilters(IQueryable<DanceStudio> query, StudioFilterModel filter)
        {
            //���������� �� ��������
            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                query = query.Where(s => s.Name.Contains(filter.Name));
            }

            //���������� �� ������
            if (!string.IsNullOrWhiteSpace(filter.Locality))
            {
                query = query.Where(s => s.IdAddressNavigation.Locality.Contains(filter.Locality));  //��������� �����
            }

            if (!string.IsNullOrWhiteSpace(filter.SettlementArea))
            {
                query = query.Where(s => s.IdAddressNavigation.SettlementArea.Contains(filter.SettlementArea));  //�����
            }

            if (!string.IsNullOrWhiteSpace(filter.Street))
            {
                query = query.Where(s => s.IdAddressNavigation.Street.Contains(filter.Street));  //�����
            }

            //���������� �� ����� - ��� ������ ������ ������
            if (!string.IsNullOrWhiteSpace(filter.Style))
            {
                var styleId = int.Parse(filter.Style);
                query = query.Where(s => s.DanceGroups.Any(g => g.IdStyle == styleId));  
            }

            //���������� �� ���� (������������ ��������� ��� ������������ ����)
            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(s => s.Prices.Any() && s.Prices.Max(p => p.Price1) <= filter.MaxPrice.Value);
            }

            //���������� �� ������� �������
            if (filter.BeginTime.HasValue)  //������
            {
                query = query.Where(s => s.DanceGroups.Any(g =>
                    g.Schedules.Any(sc => sc.BeginTime >= filter.BeginTime.Value)));
            }

            if (filter.EndTime.HasValue)  //���������
            {
                query = query.Where(s => s.DanceGroups.Any(g =>
                    g.Schedules.Any(sc => sc.EndTime <= filter.EndTime.Value)));
            }

            //���������� �� ��������
            if (filter.Age.HasValue)
            {
                int age = filter.Age.Value;
                query = query.Where(s => s.DanceGroups.Any(g =>
                    (!g.IdAgeLimitNavigation.MinAge.HasValue || g.IdAgeLimitNavigation.MinAge.Value <= age) &&
                    (!g.IdAgeLimitNavigation.MaxAge.HasValue || g.IdAgeLimitNavigation.MaxAge.Value >= age)
                ));
            }
            return query;
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
            ModelState.Remove("StudioFilter");  //������� ������ ��� ����������

            if (!ModelState.IsValid)  //���� �� �������� ���������
            {
                model.DanceStudios = _context.DanceStudios.ToList();  //������ ������
                return View("Index", model);  //�� ������� �������� � ��������� �����
            }

            //���������� ������ ��� ���������� ��������
            var styles = _context.Styles.OrderBy(s => s.IdStyle).ToList();  //�����

            var studioFilterModel = new StudioFilterModel  //�������������
            {
                Styles = styles
            };
            model.StudioFilter = studioFilterModel;

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
            /*var register = model.Register;
            var danceStudios = model.DanceStudios;
            var studioFilter = model.StudioFilter;*/
            ModelState.Remove("Register");  //������� �������� �����������
            ModelState.Remove("DanceStudios");  //������� �������� ������������ ������
            ModelState.Remove("StudioFilter");  //������� ������ ��� ����������

            if (!ModelState.IsValid)
            {
                model.DanceStudios = _context.DanceStudios.ToList();  //������ ������
                return View("Index", model);
            }

            //���������� ������ ��� ���������� ��������
            var styles = _context.Styles.OrderBy(s => s.IdStyle).ToList();  //�����

            var studioFilterModel = new StudioFilterModel  //�������������
            {
                Styles = styles
            };
            model.StudioFilter = studioFilterModel;

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
