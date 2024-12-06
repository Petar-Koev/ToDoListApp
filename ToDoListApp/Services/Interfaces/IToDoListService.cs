using ToDoListApp.Data;
using ToDoListApp.Models;

namespace ToDoListApp.Services.Interfaces
{
    public interface IToDoListService
    {
        Task<List<ToDoListInfoViewModel>> GetListsForUserAsync(string userId);
        Task AddListAsync(ToDoListViewModel list, string userId);
        Task<ToDoList> GetListByIdAsync(int id);
        Task UpdateListAsync(ToDoListViewModel list);
        Task DeleteListAsync(int id);
        Task<string> GetListNameByIdAsync(int id);
    }
}
