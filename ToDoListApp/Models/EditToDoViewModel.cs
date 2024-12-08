using System.ComponentModel.DataAnnotations;
using ToDoListApp.Enums;
using static ToDoListApp.Constants.EntityValidationConstants;

namespace ToDoListApp.Models
{
    public class EditToDoViewModel
    {
        public int Id { get; set; }

        public int ListId { get; set; }

        [Required]
        [MaxLength(EntityNameMaxLength)]
        [MinLength(EntityNameMinLength)]
        public string Name { get; set; } = null!;

        public DateTime? DueDate { get; set; }

        [Required]
        public Priority Priority { get; set; }
    }
}
