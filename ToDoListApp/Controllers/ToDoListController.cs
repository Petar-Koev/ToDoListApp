using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
            var userId = _userManager.GetUserId(User) ?? string.Empty; ;
            var lists = await _toDoListService.GetListsForUserAsync(userId);
            return View(lists);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryService.GetAllCategoriesAsync(); 
            var viewModel = new CreateToDoListViewModel
            {
                Categories = categories
            };

            return View(viewModel);
        }
    }
}
