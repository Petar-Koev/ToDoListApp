using Microsoft.AspNetCore.Mvc;
using ToDoListApp.Models;
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
        public async Task<IActionResult> Index(int todoId, int listId)
        {
            var subtasks = await _subtaskService.GetSubtasksByTodoIdAsync(todoId);
            ViewData["TodoId"] = todoId;
            ViewData["ListId"] = listId;
            return View(subtasks);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveSubtasks(int todoId, int listId,List<SubtaskViewModel> tasks)
        {

            await _subtaskService.SaveSubtasksAsync(todoId, tasks);
            return RedirectToAction("Index", "ToDo", new { listId });

        }

    }
}
