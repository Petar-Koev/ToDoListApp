using ToDoListApp.Data;
using ToDoListApp.Models;

namespace ToDoListApp.Repositories.Interfaces
{
    public interface ISubtaskRepository
    {
        Task<List<Subtask>> GetSubtasksByTodoIdAsync(int todoId);
        Task AddAsync(Subtask subtask);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();

    }
}
