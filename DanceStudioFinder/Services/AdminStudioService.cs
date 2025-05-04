using DanceStudioFinder.Data;
using DanceStudioFinder.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DanceStudioFinder.Services
{
    public class AdminStudioService
    {
        private readonly ApplicationDbContext _context;

        public AdminStudioService(ApplicationDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Извоечение админа по id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Admin?> FindAdmin(int id)
        {
            return await _context.Admins.FirstOrDefaultAsync(a => a.IdAdmin == id);
        }


        /// <summary>
        /// Нахождение студии по id её администратора
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<DanceStudio?> FindStudio(int id)
        {
            return await _context.DanceStudios.FirstOrDefaultAsync<DanceStudio>(s => s.IdAdmin == id);
        }


        /// <summary>
        /// Выгрузка таблицы с днями недели из БД
        /// </summary>
        /// <returns></returns>
        public List<WeekDay> GetWeekDays()
        {
            return _context.WeekDays.ToList();
        }


        /// <summary>
        /// Выгрузка танцевальных стилей из БД
        /// </summary>
        /// <returns></returns>
        public List<Style> GetStyles()
        {
            return _context.Styles.OrderBy(s => s.IdStyle).ToList();
        }


        /// <summary>
        /// Сохранение адреса и студии в БД
        /// </summary>
        /// <param name="address"></param>
        /// <param name="studio"></param>
        /// <returns></returns>
        public async Task<bool> CreateAddressStudio(Address address, DanceStudio studio)
        {
            return false;
        }
    }
}
