using ToDoListApp.Data;
using ToDoListApp.Repositories.Interfaces;
using ToDoListApp.Services.Interfaces;

namespace ToDoListApp.Services
{
    public class AchievementService : IAchievementService
    {
        private readonly IAchievementRepository _repository;

        public AchievementService(IAchievementRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Achievement>> GetAvailableAchievementsAsync(string userId)
        {
            return await _repository.GetAvailableAchievementsAsync(userId);
        }

        public async Task<List<Achievement>> GetUserAchievementsAsync(string userId)
        {
            return await _repository.GetUserAchievementsAsync(userId);
        }

        public async Task<Achievement?> GetAchievementByNameAsync(string name)
        {
            return await _repository.GetAchievementByNameAsync(name);
        }

        public async Task AwardAchievementAsync(string userId, int achievementId)
        {

            var userAchievements = await _repository.GetUserAchievementsAsync(userId);
            if (userAchievements.Any(a => a.Id == achievementId))
            {
                return;
            }

            var userAchievement = new UserAchievement
            {
                UserId = userId,
                AchievementId = achievementId
            };

            await _repository.AddUserAchievementAsync(userAchievement);
        }

    }
}
