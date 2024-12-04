using System.Collections.Generic;
using ToDoListApp.Data;

namespace ToDoListApp.Repositories.Interfaces
{
    public interface IToDoListRepository
    {
        Task<List<ToDoList>> GetListsByUserAsync(string userId);
    }
}
