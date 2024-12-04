using System.ComponentModel.DataAnnotations;

namespace ToDoListApp.Models
{
    public class ToDoListViewModel
    {
        public int Id { get; set; }

        [Display(Name = "List Name")]
        public string Name { get; set; } = null!;

        [Display(Name = "Category")]
        public string CategoryName { get; set; } = null!;

    }
}
