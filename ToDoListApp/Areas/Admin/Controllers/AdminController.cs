using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ToDoListApp.Areas.Admin.Models;
using ToDoListApp.Services.Interfaces;

namespace ToDoListApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IToDoListService _toDoListService;

        public AdminController(UserManager<IdentityUser> userManager, IToDoListService toDoListService)
        {
            _userManager = userManager;
            _toDoListService = toDoListService;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var userListCounts = new List<AdminDashboardViewModel>();

            foreach (var user in users)
            {
                var listCount = await _toDoListService.GetListCountByUserAsync(user.Id);
                userListCounts.Add(new AdminDashboardViewModel
                {
                    UserId = user.Id,
                    Email = user.Email ?? "",
                    ListCount = listCount
                });
            }

            return View(userListCounts);
        }
    }
}
