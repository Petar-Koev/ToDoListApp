using Microsoft.EntityFrameworkCore;
using ToDoListApp.Data;
using ToDoListApp.Repositories.Interfaces;

namespace ToDoListApp.Repositories
{
    public class AchievementRepository : IAchievementRepository
    {
        private readonly AppDbContext _context;

        public AchievementRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Achievement>> GetUserAchievementsAsync(string userId)
        {
            return await _context.UserAchievements
                .Where(ua => ua.UserId == userId)
                .Select(ua => ua.Achievement)
                .ToListAsync();
        }

        public async Task<List<Achievement>> GetAvailableAchievementsAsync(string userId)
        {
            return await _context.Achievements
                .Where(a => !_context.UserAchievements
                    .Any(ua => ua.AchievementId == a.Id && ua.UserId == userId))
                .ToListAsync();
        }

        public async Task<Achievement?> GetAchievementByNameAsync(string name)
        {
            return await _context.Achievements.FirstOrDefaultAsync(a => a.Name == name);
        }

        public async Task AddUserAchievementAsync(UserAchievement userAchievement)
        {
            await _context.UserAchievements.AddAsync(userAchievement);
            await _context.SaveChangesAsync();
        }



    }
}
