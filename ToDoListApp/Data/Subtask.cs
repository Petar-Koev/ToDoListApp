using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static ToDoListApp.Constants.EntityValidationConstants;

namespace ToDoListApp.Data
{
    public class Subtask
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(EntityNameMaxLength)]
        [MinLength(EntityNameMinLength)]
        public string Name { get; set; } = null!;

        [Required]
        public bool IsCompleted { get; set; }

        [Required]
        public int TodoId { get; set; }

        [ForeignKey(nameof(TodoId))]
        public ToDo Todo { get; set; } = null!;
    }
}
