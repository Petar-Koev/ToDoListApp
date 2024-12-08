using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoListApp.Areas.Admin.Models;
using ToDoListApp.Services.Interfaces;

namespace ToDoListApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 5;
            var users = await _adminService.GetPaginatedUsersWithListCountsAsync(page, pageSize);
            var totalUsers = await _adminService.GetTotalUsersCountAsync();

            var model = new PaginatedUsersViewModel
            {
                Users = users,
                TotalPages = (int)Math.Ceiling(totalUsers / (double)pageSize),
                CurrentPage = page
            };

            return View(model);
        }
    }
}
