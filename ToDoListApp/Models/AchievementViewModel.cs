using ToDoListApp.Data;

namespace ToDoListApp.Models
{
    public class AchievementViewModel
    {
        public List<Achievement> Achieved { get; set; } = new();
        public List<Achievement> Available { get; set; } = new();
    }
}
