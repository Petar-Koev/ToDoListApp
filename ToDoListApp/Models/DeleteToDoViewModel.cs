namespace ToDoListApp.Models
{
    public class DeleteToDoViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int ListId { get; set; }
    }
}
