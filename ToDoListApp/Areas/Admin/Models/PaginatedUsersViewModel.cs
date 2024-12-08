namespace ToDoListApp.Areas.Admin.Models
{
    public class PaginatedUsersViewModel
    {
        public List<AdminDashboardViewModel> Users { get; set; } = new();
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
