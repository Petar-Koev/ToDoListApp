using System.ComponentModel.DataAnnotations;
using static ToDoListApp.Constants.EntityValidationConstants;

namespace ToDoListApp.Models
{
    public class TaskViewModel
    {
        [Required]
        [MaxLength(EntityNameMaxLength)]
        [MinLength(EntityNameMinLength)]
        public string Name { get; set; } = null!;
    }
}
