using ToDoListApp.Data;
using ToDoListApp.Exceptions;
using ToDoListApp.Models;
using ToDoListApp.Repositories;
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
                Priority = t.Priority,
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

        public async Task AddToDoAsync(AddToDoViewModel model)
        {
            // Checking if the list is valid
            await _toDoListService.GetListByIdAsync(model.ListId);
            var isDuplicateName = await _repository.ToDoNameExistsAsync(model.Name, model.ListId);

            if (isDuplicateName)
            {
                throw new ArgumentException("A ToDo with the same name already exists and is not completed.");
            }

            var todo = new ToDo
            {
                Name = model.Name,
                DueDate = model.DueDate,
                Priority = model.Priority,
                ListId = model.ListId,
                Subtasks = model.Tasks.Select(t => new Subtask
                {
                    Name = t.Name,
                    IsCompleted = false
                }).ToList()
            };

            await _repository.AddToDoAsync(todo);
        }

        public async Task MarkAsCheckedAsync(int id)
        {
            var todo = await _repository.GetToDoByIdAsync(id);
            if (todo == null)
            {
                throw new NotFoundException($"ToDo with ID {id} not found.");
            }

            if (todo.Subtasks.Any() && todo.Subtasks.Any(subtask => !subtask.IsCompleted))
            {
                throw new InvalidOperationException("Cannot check the ToDo until all subtasks are completed.");
            }

            todo.IsCompleted = true;
            await _repository.UpdateToDoAsync(todo);
        }

        public async Task MarkAsUncheckedAsync(int id)
        {
            var todo = await _repository.GetToDoByIdAsync(id);
            if (todo == null)
            {
                throw new NotFoundException($"ToDo with ID {id} not found.");
            }

            todo.IsCompleted = false;
            await _repository.UpdateToDoAsync(todo);
        }

        public async Task UpdateToDoAsync(EditToDoViewModel model)
        {
            var todo = await _repository.GetToDoByIdAsync(model.Id);
            if (todo == null)
            {
                throw new NotFoundException($"ToDo with ID {model.Id} not found.");
            }

            todo.Name = model.Name;
            todo.DueDate = model.DueDate;
            todo.Priority = model.Priority;

            await _repository.UpdateToDoAsync(todo);
        }

        public async Task<ToDo> GetToDoByIdAsync(int id)
        {
            var todo = await _repository.GetToDoByIdAsync(id);
            if (todo == null)
            {
                throw new NotFoundException($"ToDo with ID {id} not found.");
            }
            return todo;
        }

        public async Task DeleteToDoAsync(int id)
        {
            var todo = await _repository.GetToDoByIdAsync(id);
            if (todo == null)
            {
                throw new NotFoundException($"ToDo with ID {id} not found.");
            }

            await _repository.DeleteToDoAsync(todo);
        }




    }
}
