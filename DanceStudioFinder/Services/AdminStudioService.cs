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
    }
}
