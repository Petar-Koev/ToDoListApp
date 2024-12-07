using ToDoListApp.Models;
using ToDoListApp.Repositories.Interfaces;
using ToDoListApp.Services.Interfaces;

namespace ToDoListApp.Services
{
    public class SubtaskService : ISubtaskService
    {

        private readonly ISubtaskRepository _subtaskRepository;

        public SubtaskService(ISubtaskRepository subtaskRepository)
        {
            _subtaskRepository = subtaskRepository;
        }

        public async Task<List<SubtaskViewModel>> GetSubtasksByTodoIdAsync(int todoId)
        {
            var subtasks = await _subtaskRepository.GetSubtasksByTodoIdAsync(todoId);

            return subtasks.Select(s => new SubtaskViewModel
            {
                Id = s.Id,
                Name = s.Name,
                IsCompleted = s.IsCompleted
            }).ToList();
        }

    }
}
