using ToDoListApp.Data;
using ToDoListApp.Models;

namespace ToDoListApp.Services.Interfaces
{
    public interface IToDoService
    {
        Task<OpenListViewModel> GetTodosByListIdAsync(int listId);
        Task AddToDoAsync(AddToDoViewModel model);
        Task MarkAsCheckedAsync(int id);
        Task MarkAsUncheckedAsync(int id);
        Task UpdateToDoAsync(EditToDoViewModel model);
        Task<ToDo> GetToDoByIdAsync(int id);
        Task DeleteToDoAsync(int id);
    }
}
