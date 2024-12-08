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

        public async Task<ToDo?> GetToDoByIdAsync(int id)
        {
            return await _context.Todos
        .Include(todo => todo.Subtasks) 
        .FirstOrDefaultAsync(todo => todo.Id == id);
        }

        public async Task UpdateToDoAsync(ToDo todo)
        {
            _context.Todos.Update(todo);
            await _context.SaveChangesAsync();
        }

        public async Task<ToDo?> GetToDoByIdWithSubtasksAsync(int id)
        {
            return await _context.Todos
                .Include(todo => todo.Subtasks)
                .FirstOrDefaultAsync(todo => todo.Id == id);
        }

        public async Task DeleteToDoAsync(ToDo todo)
        {
            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();
        }


    }
}
