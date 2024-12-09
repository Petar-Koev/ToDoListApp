using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ToDoListApp.Exceptions;
using ToDoListApp.Models;
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
        public async Task<IActionResult> Index(int listId, string sortOrder = "default")
        {
            try
            {
                bool sortByPriority = sortOrder.Equals("priority");

                var viewModel = await _toDoService.GetTodosByListIdAsync(listId, sortByPriority);

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Check(int todoId, int listId)
        {
            try
            {
                await _toDoService.MarkAsCheckedAsync(todoId);
                return RedirectToAction("Index", new { listId });
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", new { listId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Uncheck(int todoId, int listId)
        {
            try
            {
                await _toDoService.MarkAsUncheckedAsync(todoId);
                return RedirectToAction("Index", new { listId });
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while unchecking the ToDo.";
                return RedirectToAction("Index", new { listId });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var todo = await _toDoService.GetToDoByIdAsync(id);
                var model = new EditToDoViewModel
                {
                    Id = todo.Id,
                    Name = todo.Name,
                    DueDate = todo.DueDate,
                    Priority = todo.Priority,
                    ListId = todo.ListId
                };

                return View(model);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditToDoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _toDoService.UpdateToDoAsync(model);
                return RedirectToAction("Index", new { listId = model.ListId });
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var todo = await _toDoService.GetToDoByIdAsync(id);
                var model = new DeleteToDoViewModel
                {
                    Id = todo.Id,
                    Name = todo.Name,
                    ListId = todo.ListId
                };

                return View(model);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int listId)
        {
            try
            {
                await _toDoService.DeleteToDoAsync(id);
                return RedirectToAction("Index", new { listId } );
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }
    }
}
