using ToDoListApp.Data;

namespace ToDoListApp.Repositories.Interfaces
{
    public interface IAchievementRepository
    {
        Task<List<Achievement>> GetUserAchievementsAsync(string userId);
        Task<List<Achievement>> GetAvailableAchievementsAsync(string userId);
        Task<Achievement?> GetAchievementByNameAsync(string name);
        Task AddUserAchievementAsync(UserAchievement userAchievement);


    }
}
