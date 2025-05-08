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

    }
}
