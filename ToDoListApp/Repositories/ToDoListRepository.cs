using Microsoft.EntityFrameworkCore;
using ToDoListApp.Data;
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
    }
}
