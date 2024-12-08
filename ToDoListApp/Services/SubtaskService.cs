using ToDoListApp.Data;
using ToDoListApp.Exceptions;
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

        public async Task SaveSubtasksAsync(int todoId, List<SubtaskViewModel> tasks)
        {
            ValidateTasks(tasks);

            var existingSubtasks = await _subtaskRepository.GetSubtasksByTodoIdAsync(todoId);

            // Update existing subtasks
            foreach (var task in tasks.Where(t => t.Id > 0))
            {
                var existingSubtask = existingSubtasks.FirstOrDefault(s => s.Id == task.Id);
                if (existingSubtask != null)
                {
                    existingSubtask.IsCompleted = task.IsCompleted;
                }
            }

            // Add new tasks
            foreach (var task in tasks.Where(t => t.Id == 0))
            {
                var newSubtask = new Subtask
                {
                    Name = task.Name,
                    IsCompleted = task.IsCompleted,
                    TodoId = todoId
                };
                await _subtaskRepository.AddAsync(newSubtask);
            }

            // Handle removed tasks
            var submittedIds = tasks.Where(t => t.Id > 0).Select(t => t.Id).ToList();
            var tasksToRemove = existingSubtasks.Where(s => !submittedIds.Contains(s.Id)).ToList();

            foreach (var taskToRemove in tasksToRemove)
            {
                await _subtaskRepository.DeleteAsync(taskToRemove.Id);
            }

            await _subtaskRepository.SaveChangesAsync();
        }

        private void ValidateTasks(List<SubtaskViewModel> tasks)
        {
            foreach (var task in tasks)
            {
                if (string.IsNullOrWhiteSpace(task.Name))
                {
                    throw new ArgumentException("Task name cannot be empty.");
                }

                if (task.Name.Length < 2 || task.Name.Length > 100)
                {
                    throw new ArgumentException($"Task name '{task.Name}' must be between 2 and 100 characters.");
                }
            }

            var duplicateNames = tasks.GroupBy(t => t.Name)
                                       .Where(g => g.Count() > 1)
                                       .Select(g => g.Key)
                                       .ToList();

            if (duplicateNames.Any())
            {
                throw new ArgumentException("Duplicate tasks detected");
            }
        }
    }
}
