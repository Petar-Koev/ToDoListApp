
namespace ToDoListApp.Models
{
    public class SubtaskViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsCompleted { get; set; }
    }
}
