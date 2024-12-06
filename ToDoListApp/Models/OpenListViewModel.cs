using ToDoListApp.Data;

namespace ToDoListApp.Models
{
    public class OpenListViewModel
    {
        public int ListId { get; set; }
        public string ListName { get; set; } = null!;
        public IEnumerable<ToDoViewModel> Todos { get; set; } = new List<ToDoViewModel>();
    }
}
