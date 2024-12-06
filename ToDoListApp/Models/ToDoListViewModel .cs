using System.ComponentModel.DataAnnotations;
using ToDoListApp.Data;
using static ToDoListApp.Constants.EntityValidationConstants;

namespace ToDoListApp.Models
{
    public class ToDoListViewModel
    {
        public int? Id { get; set; }

        [Required]
        [MaxLength(EntityNameMaxLength)]
        [MinLength(EntityNameMinLength)]
        public string Name { get; set; } = null!;

        [MaxLength(EntityDescriptionMaxLength)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }

        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
    }
}
