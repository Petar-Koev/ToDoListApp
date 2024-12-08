namespace ToDoListApp.Areas.Admin.Models
{
    public class AdminDashboardViewModel
    {
        public string UserId { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int ListCount { get; set; }
    }
}
