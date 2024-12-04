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

        public async Task<List<ToDoListViewModel>> GetListsForUserAsync(string userId)
        {
            var lists = await _toDoListRepository.GetListsByUserAsync(userId);
            return lists.Select(l => new ToDoListViewModel
            {
                Id = l.Id,
                Name = l.Name,
                CategoryName = l.Category.Name
            }).ToList();
        }
}
}
