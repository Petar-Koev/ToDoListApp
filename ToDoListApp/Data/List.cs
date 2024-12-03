using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static ToDoListApp.Constants.EntityValidationConstants;

namespace ToDoListApp.Data
{
    public class List
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(EntityNameMaxLength)]
        [MinLength(EntityNameMinLength)]
        public string Name { get; set; } = null!;

        [MaxLength(EntityDescriptionMaxLength)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; } = null!;

        [Required]
        public bool IsDeleted { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        public ICollection<ToDo> Todos { get; set; } = new List<ToDo>();
    }
}
