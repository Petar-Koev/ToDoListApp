using ToDoListApp.Data;

namespace ToDoListApp.Services.Interfaces
{
    public interface IAchievementService
    {
        Task<List<Achievement>> GetAvailableAchievementsAsync(string userId);
        Task<List<Achievement>> GetUserAchievementsAsync(string userId);
        Task<Achievement?> GetAchievementByNameAsync(string name);
        Task AwardAchievementAsync(string userId, int achievementId);
    }
}

