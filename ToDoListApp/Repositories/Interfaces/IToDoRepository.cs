using ToDoListApp.Data;

namespace ToDoListApp.Repositories.Interfaces
{
    public interface IToDoRepository
    {
        Task<List<ToDo>> GetTodosByListIdAsync(int listId);
    }
}
