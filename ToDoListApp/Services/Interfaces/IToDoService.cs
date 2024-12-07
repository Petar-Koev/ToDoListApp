using ToDoListApp.Models;

namespace ToDoListApp.Services.Interfaces
{
    public interface IToDoService
    {
        Task<OpenListViewModel> GetTodosByListIdAsync(int listId);
        Task AddToDoAsync(AddToDoViewModel model);
    }
}
