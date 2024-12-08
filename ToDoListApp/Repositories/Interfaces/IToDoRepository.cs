using ToDoListApp.Data;

namespace ToDoListApp.Repositories.Interfaces
{
    public interface IToDoRepository
    {
        Task<List<ToDo>> GetTodosByListIdAsync(int listId);
        Task AddToDoAsync(ToDo todo);
        Task<bool> ToDoNameExistsAsync(string name, int listId);
        Task UpdateToDoAsync(ToDo todo);
        Task<ToDo?> GetToDoByIdAsync(int id);
        Task<ToDo?> GetToDoByIdWithSubtasksAsync(int id);
        Task DeleteToDoAsync(ToDo todo);

    }
}
