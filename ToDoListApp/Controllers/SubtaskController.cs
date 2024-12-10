using Microsoft.AspNetCore.Mvc;
using ToDoListApp.Exceptions;
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
            try
            {
                var subtasks = await _subtaskService.GetSubtasksByTodoIdAsync(todoId);

                ViewData["TodoId"] = todoId;
                ViewData["ListId"] = listId;
                return View(subtasks);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveSubtasks(int todoId, int listId,List<SubtaskViewModel> tasks)
        {
            try
            {
                await _subtaskService.SaveSubtasksAsync(todoId, tasks);
                return RedirectToAction("Index", "ToDo", new { listId });
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
        }
    }
}
