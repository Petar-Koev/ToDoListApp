using Microsoft.EntityFrameworkCore;
using ToDoListApp.Data;
using ToDoListApp.Repositories.Interfaces;

namespace ToDoListApp.Repositories
{
    public class SubtaskRepository : ISubtaskRepository
    {
        private readonly AppDbContext _context;

        public SubtaskRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Subtask>?> GetSubtasksByTodoIdAsync(int todoId)
        {
            return await _context.Subtasks
                .Where(s => s.TodoId == todoId)
                .ToListAsync();
        }

        public async Task AddAsync(Subtask subtask)
        {
            _context.Subtasks.Add(subtask);
            await SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var subtask = await _context.Subtasks.FindAsync(id);
            if (subtask != null)
            {
                _context.Subtasks.Remove(subtask);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}
