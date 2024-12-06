using Microsoft.AspNetCore.Mvc;
using ToDoListApp.Exceptions;
using ToDoListApp.Services.Interfaces;

namespace ToDoListApp.Controllers
{
    public class ToDoController : Controller
    {
        private readonly IToDoService _toDoService;
        public ToDoController(IToDoService toDoService)
        {
            _toDoService = toDoService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int listId)
        {
            try
            {
                var viewModel = await _toDoService.GetTodosByListIdAsync(listId);
                return View(viewModel);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }
    }
}
