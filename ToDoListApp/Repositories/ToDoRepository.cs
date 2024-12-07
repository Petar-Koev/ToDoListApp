using Microsoft.EntityFrameworkCore;
using ToDoListApp.Data;
using ToDoListApp.Repositories.Interfaces;

namespace ToDoListApp.Repositories
{
    public class ToDoRepository : IToDoRepository
    {
        private readonly AppDbContext _context;

        public ToDoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ToDo>> GetTodosByListIdAsync(int listId)
        {
            return await _context.Todos
                .Where(t => t.ListId == listId)
                .Include(t => t.Subtasks) 
                .ToListAsync();
        }

        public async Task AddToDoAsync(ToDo todo)
        {
            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ToDoNameExistsAsync(string name, int listId)
        {
            return await _context.Todos
                .AnyAsync(t => t.Name == name && t.ListId == listId && !t.IsCompleted);
        }
    }
}
