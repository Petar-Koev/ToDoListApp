using ToDoListApp.Models;

namespace ToDoListApp.Services.Interfaces
{
    public interface ISubtaskService
    {
        Task<List<SubtaskViewModel>> GetSubtasksByTodoIdAsync(int todoId);
        Task SaveSubtasksAsync(int todoId, List<SubtaskViewModel> tasks);

    }
}
