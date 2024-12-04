using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ToDoListApp.Services.Interfaces;

namespace ToDoListApp.Controllers
{
    public class ToDoListController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IToDoListService _toDoListService;

        public ToDoListController(UserManager<IdentityUser> userManager, IToDoListService toDoListService)
        {
            _userManager = userManager;
            _toDoListService = toDoListService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User) ?? string.Empty; ;
            var lists = await _toDoListService.GetListsForUserAsync(userId);
            return View(lists);
        }
    }
}
