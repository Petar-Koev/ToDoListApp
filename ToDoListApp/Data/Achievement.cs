using System.ComponentModel.DataAnnotations;
using static ToDoListApp.Constants.EntityValidationConstants;

namespace ToDoListApp.Data
{
    public class Achievement
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(EntityNameMaxLength)]
        [MinLength(EntityNameMinLength)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(EntityDescriptionMaxLength)]
        public string Description { get; set; } = string.Empty;

        public ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();
    }
}
