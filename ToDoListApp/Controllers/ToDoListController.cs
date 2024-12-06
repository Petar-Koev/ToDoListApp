using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ToDoListApp.Data;
using ToDoListApp.Models;
using ToDoListApp.Services.Interfaces;

namespace ToDoListApp.Controllers
{
    public class ToDoListController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IToDoListService _toDoListService;
        private readonly ICategoryService _categoryService;

        public ToDoListController(UserManager<IdentityUser> userManager, IToDoListService toDoListService, ICategoryService categoryService)
        {
            _userManager = userManager;
            _toDoListService = toDoListService;
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User) ?? string.Empty; 
            var lists = await _toDoListService.GetListsForUserAsync(userId);
            return View(lists);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryService.GetAllCategoriesAsync(); 
            var viewModel = new ToDoListViewModel
            {
                Categories = categories
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ToDoListViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await _categoryService.GetAllCategoriesAsync(); 
                return View(model);
            }

            var userId = _userManager.GetUserId(User) ?? string.Empty; 
            await _toDoListService.AddListAsync(model, userId);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var list = await _toDoListService.GetListByIdAsync(id);
            if (list == null)
            {
                return NotFound();
            }

            var categories = await _categoryService.GetAllCategoriesAsync();

            var viewModel = new ToDoListViewModel
            {
                Id = list.Id,
                Name = list.Name,
                Description = list.Description,
                CategoryId = list.CategoryId,
                Categories = categories
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ToDoListViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await _categoryService.GetAllCategoriesAsync(); 
                return View(model);
            }

            try
            {
                await _toDoListService.UpdateListAsync(model);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var list = await _toDoListService.GetListByIdAsync(id);
            if (list == null)
            {
                return NotFound();
            }

            var viewModel = new ToDoListViewModel
            {
                Id = list.Id,
                Name = list.Name,
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _toDoListService.DeleteListAsync(id);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

    }
}
