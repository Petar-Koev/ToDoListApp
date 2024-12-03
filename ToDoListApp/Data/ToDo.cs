using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ToDoListApp.Enums;
using static ToDoListApp.Constants.EntityValidationConstants;

namespace ToDoListApp.Data
{
    public class ToDo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(EntityNameMaxLength)]
        [MinLength(EntityNameMinLength)]
        public string Name { get; set; } = null!;

        public DateTime? DueDate { get; set; }

        [Required]
        public Priority Priority { get; set; } 

        [Required]
        public bool IsCompleted { get; set; }

        [Required]
        public int ListId { get; set; }

        [ForeignKey(nameof(ListId))]
        public List List { get; set; } = null!;

        public ICollection<Subtask> Subtasks { get; set; } = new List<Subtask>();
    }
}
