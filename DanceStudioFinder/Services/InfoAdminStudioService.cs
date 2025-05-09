using DanceStudioFinder.Data;
using DanceStudioFinder.Models;
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


        public async Task<Address?> FindAddress(int addressId)
        {
            return await _context.Addresses.FirstOrDefaultAsync<Address>(a => a.IdAddress == addressId);
        }

        public async Task UpdateStudio(DanceStudio studio)
        {
            _context.DanceStudios.Update(studio);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateAddress(Address address)
        {
            _context.Addresses.Update(address);
            await _context.SaveChangesAsync();
        }

        public async Task<DanceStudio?> FindStudioWithPrices(int id)
        {
            return await _context.DanceStudios
                .Include(s => s.Prices) // включаем связанные цены
                .FirstOrDefaultAsync(s => s.IdAdmin == id);
        }

        public async Task<bool> DeleteAllPricesForStudio(int studioId)
        {
            try
            {
                var pricesToDelete = await _context.Prices
                    .Where(p => p.IdStudio == studioId)
                    .ToListAsync();

                _context.Prices.RemoveRange(pricesToDelete);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<GroupWithSchedules>> GetGroupsWithSchedules(int studioId)
        {
            return await _context.DanceGroups
                .Where(g => g.IdStudio == studioId)
                .Include(g => g.Schedules)
                .Include(g => g.IdAgeLimitNavigation) // Добавляем загрузку AgeLimit
                .Select(g => new GroupWithSchedules
                {
                    Id = g.IdGroup,
                    Name = g.Name,
                    StyleId = g.IdStyle,
                    // Берем MinAge и MaxAge из связанной таблицы AgeLimits
                    MinAge = g.IdAgeLimitNavigation != null ? g.IdAgeLimitNavigation.MinAge : null,
                    MaxAge = g.IdAgeLimitNavigation != null ? g.IdAgeLimitNavigation.MaxAge : null,
                    Description = g.Description,
                    Schedules = g.Schedules.Select(s => new Schedule
                    {
                        IdSchedule = s.IdSchedule,
                        IdDay = s.IdDay,
                        BeginTime = s.BeginTime,
                        EndTime = s.EndTime,
                        IdGroup = s.IdGroup
                    }).ToList()
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<DanceGroup>> GetGroupsByStudioId(int studioId)
        {
            return await _context.DanceGroups
                .Where(g => g.IdStudio == studioId)
                .ToListAsync();
        }

        public async Task<HashSet<int>> GetAgeLimitsUsedByOtherStudios(int studioId)
        {
            var ageLimits = await _context.DanceGroups
                .Where(g => g.IdStudio != studioId)
                .Select(g => g.IdAgeLimit)
                .Distinct()
                .ToListAsync();

            return new HashSet<int>(ageLimits);
        }

        public async Task DeleteSchedulesByGroupId(int groupId)
        {
            var schedules = await _context.Schedules
                .Where(s => s.IdGroup == groupId)
                .ToListAsync();

            _context.Schedules.RemoveRange(schedules);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteGroup(int groupId)
        {
            var group = await _context.DanceGroups.FindAsync(groupId);
            if (group != null)
            {
                _context.DanceGroups.Remove(group);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAgeLimit(int ageLimitId)
        {
            var ageLimit = await _context.AgeLimits.FindAsync(ageLimitId);
            if (ageLimit != null)
            {
                _context.AgeLimits.Remove(ageLimit);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsAgeLimitUsedByOtherGroups(int ageLimitId, int excludeGroupId)
        {
            return await _context.DanceGroups
                .AnyAsync(g => g.IdAgeLimit == ageLimitId && g.IdGroup != excludeGroupId);
        }
    }
    public class GroupWithSchedules
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StyleId { get; set; }
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
        public string Description { get; set; }
        public List<Schedule> Schedules { get; set; }
    }
}
