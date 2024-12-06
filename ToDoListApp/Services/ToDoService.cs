using ToDoListApp.Data;
using ToDoListApp.Models;
using ToDoListApp.Repositories.Interfaces;
using ToDoListApp.Services.Interfaces;

namespace ToDoListApp.Services
{
    public class ToDoService : IToDoService
    {
        private readonly IToDoRepository _repository;
        private readonly IToDoListService _toDoListService;

        public ToDoService(IToDoRepository repository, IToDoListService toDoListService)
        {
            _repository = repository;
            _toDoListService = toDoListService;
        }

        public async Task<OpenListViewModel> GetTodosByListIdAsync(int listId)
        {
            var todos = await _repository.GetTodosByListIdAsync(listId);
            var listName = await _toDoListService.GetListNameByIdAsync(listId);
            var model = todos.Select(t => new ToDoViewModel
            {
                Id = t.Id,
                Name = t.Name,
                IsChecked = t.IsCompleted,
                Subtasks = t.Subtasks.Select(s => new SubtaskViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    IsCompleted = s.IsCompleted
                }).ToList()
            });
            var viewModel = new OpenListViewModel
            {
                ListId = listId,
                ListName = listName,
                Todos = model
            };
            return viewModel;
        }
    }
}
