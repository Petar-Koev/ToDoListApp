using ToDoListApp.Models;

namespace ToDoListApp.Services.Interfaces
{
    public interface IToDoListService
    {
        Task<List<ToDoListViewModel>> GetListsForUserAsync(string userId);
    }
}
