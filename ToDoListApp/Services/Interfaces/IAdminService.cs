using ToDoListApp.Areas.Admin.Models;

namespace ToDoListApp.Services.Interfaces
{
    public interface IAdminService
    {
        Task<List<AdminDashboardViewModel>> GetPaginatedUsersWithListCountsAsync(int page, int pageSize);
        Task<int> GetTotalUsersCountAsync();

    }
}
