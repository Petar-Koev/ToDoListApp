using ToDoListApp.Enums;

namespace ToDoListApp.Models
{
    public class ToDoViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsChecked { get; set; }
        public Priority Priority { get; set; }
        public ICollection<SubtaskViewModel> Subtasks { get; set; } = new List<SubtaskViewModel>();
    }
}
