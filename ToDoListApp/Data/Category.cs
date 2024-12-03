using System.ComponentModel.DataAnnotations;
using static ToDoListApp.Constants.EntityValidationConstants;

namespace ToDoListApp.Data
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(EntityNameMaxLength)]
        [MinLength(EntityNameMinLength)]
        public string Name { get; set; } = null!;

        public ICollection<List> Lists { get; set; } = new List<List>();
    }
}
