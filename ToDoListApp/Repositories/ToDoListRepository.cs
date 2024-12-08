using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using ToDoListApp.Data;
using ToDoListApp.Exceptions;
using ToDoListApp.Repositories.Interfaces;

namespace ToDoListApp.Repositories
{
    public class ToDoListRepository : IToDoListRepository
    {
        private readonly AppDbContext _context;

        public ToDoListRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ToDoList>> GetListsByUserAsync(string userId)
        {
            return await _context.ToDoLists
                .Where(l => l.UserId == userId && !l.IsDeleted)
                .Include(l => l.Category)
                .ToListAsync();
        }

        public async Task AddListAsync(ToDoList list)
        {
            _context.ToDoLists.Add(list);
            await _context.SaveChangesAsync();
        }

        public async Task<ToDoList?> GetListByIdAsync(int id)
        {
            return await _context.ToDoLists
                .Include(l => l.Category) 
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task UpdateListAsync(ToDoList list)
        {
            _context.ToDoLists.Update(list);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ListNameExistsAsync(string name, string userId)
        {
            return await _context.ToDoLists
                .AnyAsync(l => l.Name == name && l.UserId == userId && !l.IsDeleted);
        }

        public async Task<int> GetListCountByUserAsync(string userId)
        {
            return await _context.ToDoLists.CountAsync(l => l.UserId == userId);
        }

        public async Task<bool> HasListsAsync(string userId)
        {
            return await _context.ToDoLists.AnyAsync(l => l.UserId == userId);
        }


    }
}
