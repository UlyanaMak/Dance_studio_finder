using DanceStudioFinder.Data;
using DanceStudioFinder.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

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
        /// Нахождение студии по id её администратора
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> FindAddress(Address address)
        {
            var result = await _context.Addresses
                .Where(a => a.Entity == address.Entity &&
                a.Locality == address.Locality &&
                a.Street == address.Street &&
                a.BuildingNumber == address.BuildingNumber &&
                a.Letter == address.Letter &&
                a.SettlementArea == address.SettlementArea)
                .Select(a => a.IdAddress)
                .FirstOrDefaultAsync();

            return result; // вернет 0 если адрес не найден
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
        public async Task<int> SaveAddress(Address address)
        {
            await _context.Addresses.AddAsync(address);
            await _context.SaveChangesAsync();

            // Возвращаем ID нового адреса
            return address.IdAddress;
        }

        /// <summary>
        /// Сохранение студии в БД
        /// </summary>
        /// <param name="address"></param>
        /// <param name="studio"></param>
        /// <returns></returns>
        public async Task<bool> SaveStudio(int addressId, Admin admin, DanceStudio studio)
        {
            try
            {
                // Устанавливаем связи
                studio.IdAddress = addressId;
                studio.IdAdmin = admin.IdAdmin;

                // Сохраняем студию
                await _context.DanceStudios.AddAsync(studio);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}
