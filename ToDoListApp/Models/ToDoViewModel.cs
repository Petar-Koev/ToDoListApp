namespace ToDoListApp.Models
{
    public class ToDoViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsChecked { get; set; }
        public bool HasSubtasks => Subtasks.Any();
        public ICollection<SubtaskViewModel> Subtasks { get; set; } = new List<SubtaskViewModel>();
    }
}
