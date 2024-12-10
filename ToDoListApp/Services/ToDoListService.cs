using ToDoListApp.Data;
using ToDoListApp.Exceptions;
using ToDoListApp.Models;
using ToDoListApp.Repositories.Interfaces;
using ToDoListApp.Services.Interfaces;

namespace ToDoListApp.Services
{
    public class ToDoListService : IToDoListService
    {
        private readonly IToDoListRepository _toDoListRepository;
        private readonly IAchievementService _achievementService;

        public ToDoListService(IToDoListRepository toDoListRepository, IAchievementService achievementService)
        {
            _toDoListRepository = toDoListRepository;
            _achievementService = achievementService;
        }

        public async Task<List<ToDoListInfoViewModel>> GetListsForUserAsync(string userId)
        {
            var lists = await _toDoListRepository.GetListsByUserAsync(userId);
            return lists.Select(l => new ToDoListInfoViewModel
            {
                Id = l.Id,
                Name = l.Name,
                CategoryName = l.Category.Name
            }).ToList();
        }

        public async Task AddListAsync(ToDoListViewModel list, string userId)
        {
            bool isDuplicateName = await _toDoListRepository.ListNameExistsAsync(list.Name, userId);

            if (isDuplicateName)
            {
                throw new ArgumentException("List name already exists.");
            }

            var newList = new ToDoList
            {
                Name = list.Name,
                Description = list.Description,
                CategoryId = list.CategoryId,
                UserId = userId,
                CreatedDate = DateTime.Now,
                IsDeleted = false
            };

            await SaveAchievementAsync(userId);
            await _toDoListRepository.AddListAsync(newList);
        }

        public async Task SaveAchievementAsync(string userId)
        {
            var hasOtherLists = await _toDoListRepository.HasListsAsync(userId);
            if (!hasOtherLists)
            {
                string achievementName = "First List";
                var achievement = await _achievementService.GetAchievementByNameAsync(achievementName);

                if (achievement != null)
                {
                    await _achievementService.AwardAchievementAsync(userId, achievement.Id);
                }
            }
        }

        public async Task<ToDoList> GetListByIdAsync(int id)
        {
            var list =  await _toDoListRepository.GetListByIdAsync(id);

            return list ?? throw new NotFoundException($"List with ID {id} not found.");
        }

        public async Task UpdateListAsync(ToDoListViewModel list)
        {
            if (!list.Id.HasValue)
            {
                throw new ArgumentException("List ID cannot be null.", nameof(list.Id));
            }

            var existingList = await GetListByIdAsync(list.Id.Value);

            existingList.Name = list.Name;
            existingList.Description = list.Description;
            existingList.CategoryId = list.CategoryId;

            await _toDoListRepository.UpdateListAsync(existingList);
        }

        public async Task DeleteListAsync(int id)
        {
            var existingList = await GetListByIdAsync(id);

            existingList.IsDeleted = true; 
            await _toDoListRepository.UpdateListAsync(existingList);
        }

        public async Task<string> GetListNameByIdAsync(int id)
        {
            var list = await GetListByIdAsync(id);
           
            return list.Name;
        }

        public async Task<int> GetListCountByUserAsync(string userId)
        {
            return await _toDoListRepository.GetListCountByUserAsync(userId);
        }

    }
}
