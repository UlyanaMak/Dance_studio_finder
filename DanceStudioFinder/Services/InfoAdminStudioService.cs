using DanceStudioFinder.Data;
using Microsoft.EntityFrameworkCore;

namespace DanceStudioFinder.Services
{
    public class InfoAdminStudioService
    {
        private readonly ApplicationDbContext _context;

        public InfoAdminStudioService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteStudio(int adminId)
        {
            try
            {
                var studio = await _context.DanceStudios
                    .Include(s => s.DanceGroups)
                        .ThenInclude(g => g.Schedules)
                    .FirstOrDefaultAsync(s => s.IdAdmin == adminId);

                if (studio != null)
                {
                    foreach (var group in studio.DanceGroups)
                    {
                        _context.Schedules.RemoveRange(group.Schedules);
                    }

                    _context.DanceGroups.RemoveRange(studio.DanceGroups);
                    _context.DanceStudios.Remove(studio);

                    await _context.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                // Логгируй ex, если нужно
                return false;
            }
        }

        public async Task<bool> DeleteAdmin(int adminId)
        {
            try
            {
                // Находим администратора со всеми связанными данными
                var admin = await _context.Admins
                    .Include(a => a.DanceStudios)
                        .ThenInclude(s => s.DanceGroups)
                            .ThenInclude(g => g.Schedules)
                    .FirstOrDefaultAsync(a => a.IdAdmin == adminId);

                if (admin != null)
                {
                    // Удаляем все связанные данные
                    foreach (var studio in admin.DanceStudios)
                    {
                        foreach (var group in studio.DanceGroups)
                        {
                            _context.Schedules.RemoveRange(group.Schedules);
                        }
                        _context.DanceGroups.RemoveRange(studio.DanceGroups);
                    }

                    _context.DanceStudios.RemoveRange(admin.DanceStudios);
                    _context.Admins.Remove(admin);

                    await _context.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                // _logger.LogError(ex, "Ошибка при удалении администратора");
                return false;
            }
        }

    }
}
