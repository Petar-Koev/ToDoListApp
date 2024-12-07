using Microsoft.AspNetCore.Mvc;
using ToDoListApp.Services.Interfaces;

namespace ToDoListApp.Controllers
{
    public class SubtaskController : Controller
    {
        private readonly ISubtaskService _subtaskService;

        public SubtaskController(ISubtaskService subtaskService)
        {
            _subtaskService = subtaskService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int todoId)
        {
            var subtasks = await _subtaskService.GetSubtasksByTodoIdAsync(todoId);
            return View(subtasks);
        }
    }
}
