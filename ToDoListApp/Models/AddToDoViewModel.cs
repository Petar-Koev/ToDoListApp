using System.ComponentModel.DataAnnotations;
using ToDoListApp.Enums;
using static ToDoListApp.Constants.EntityValidationConstants;

namespace ToDoListApp.Models
{
    public class AddToDoViewModel
    {
        public int ListId { get; set; }
        [Required]
        [MaxLength(EntityNameMaxLength)]
        [MinLength(EntityNameMinLength)]
        public string Name { get; set; } = null!;

        public DateTime? DueDate { get; set; }

        [Required]
        public Priority Priority { get; set; }

        public List<TaskViewModel> Tasks { get; set; } = new List<TaskViewModel>();
    }
}
