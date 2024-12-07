using Microsoft.AspNetCore.Mvc;
using ToDoListApp.Exceptions;
using ToDoListApp.Models;
using ToDoListApp.Services;
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

        [HttpGet]
        public IActionResult Add(int listId)
        {
            var viewModel = new AddToDoViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddToDoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _toDoService.AddToDoAsync(model);
                return RedirectToAction("Index", new { listId = model.ListId });
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("Name", ex.Message);
                return View(model);
            }
        }


    }
}
