﻿using System.Collections.Generic;
using ToDoListApp.Data;

namespace ToDoListApp.Repositories.Interfaces
{
    public interface IToDoListRepository
    {
        Task<List<ToDoList>> GetListsByUserAsync(string userId);
        Task AddListAsync(ToDoList list);
        Task<ToDoList?> GetListByIdAsync(int id);
        Task UpdateListAsync(ToDoList list);
    }
}
