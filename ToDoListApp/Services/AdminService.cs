using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToDoListApp.Areas.Admin.Models;
using ToDoListApp.Services.Interfaces;

namespace ToDoListApp.Services
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IToDoListService _toDoListService;

        public AdminService(UserManager<IdentityUser> userManager, IToDoListService toDoListService)
        {
            _userManager = userManager;
            _toDoListService = toDoListService;
        }

        public async Task<List<AdminDashboardViewModel>> GetPaginatedUsersWithListCountsAsync(int page, int pageSize)
        {
            var skip = (page - 1) * pageSize;

            var users = await _userManager.Users
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            var userList = new List<AdminDashboardViewModel>();

            foreach (var user in users)
            {
                var listCount = await _toDoListService.GetListCountByUserAsync(user.Id);
                userList.Add(new AdminDashboardViewModel
                {
                    UserId = user.Id,
                    Email = user.Email ?? "N/A",
                    ListCount = listCount
                });
            }

            return userList;
        }

        public async Task<int> GetTotalUsersCountAsync()
        {
            return await _userManager.Users.CountAsync();
        }
    }
}
