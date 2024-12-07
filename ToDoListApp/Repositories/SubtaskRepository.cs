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

        public async Task<List<Subtask>> GetSubtasksByTodoIdAsync(int todoId)
        {
            return await _context.Subtasks
                .Where(s => s.TodoId == todoId)
                .ToListAsync();
        }

    }
}
