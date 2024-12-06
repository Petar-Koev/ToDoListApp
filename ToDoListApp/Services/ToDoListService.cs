using ToDoListApp.Data;
using ToDoListApp.Models;
using ToDoListApp.Repositories;
using ToDoListApp.Repositories.Interfaces;
using ToDoListApp.Services.Interfaces;

namespace ToDoListApp.Services
{
    public class ToDoListService : IToDoListService
    {
        private readonly IToDoListRepository _toDoListRepository;

        public ToDoListService(IToDoListRepository toDoListRepository)
        {
            _toDoListRepository = toDoListRepository;
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
            var newList = new ToDoList
            {
                Name = list.Name,
                Description = list.Description,
                CategoryId = list.CategoryId,
                UserId = userId,
                CreatedDate = DateTime.Now,
                IsDeleted = false
            };

            await _toDoListRepository.AddListAsync(newList);
        }

        public async Task<ToDoList?> GetListByIdAsync(int id)
        {
            return await _toDoListRepository.GetListByIdAsync(id);
        }

        public async Task UpdateListAsync(ToDoListViewModel list)
        {
            if (!list.Id.HasValue)
            {
                throw new ArgumentException("List ID cannot be null.", nameof(list.Id));
            }

            var existingList = await _toDoListRepository.GetListByIdAsync(list.Id.Value);
            if (existingList == null)
            {
                throw new Exception();
            }

            existingList.Name = list.Name;
            existingList.Description = list.Description;
            existingList.CategoryId = list.CategoryId;

            await _toDoListRepository.UpdateListAsync(existingList);
        }

        public async Task DeleteListAsync(int id)
        {
            var existingList = await _toDoListRepository.GetListByIdAsync(id);
            if (existingList == null)
            {
                throw new Exception();
            }

            existingList.IsDeleted = true; 
            await _toDoListRepository.UpdateListAsync(existingList);
        }
    }
}
