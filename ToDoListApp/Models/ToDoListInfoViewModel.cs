using System.ComponentModel.DataAnnotations;

namespace ToDoListApp.Models
{
    public class ToDoListInfoViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string CategoryName { get; set; } = null!;

    }
}
