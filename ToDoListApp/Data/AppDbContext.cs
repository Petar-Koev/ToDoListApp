using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace ToDoListApp.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Category>().HasData(
              new Category { Id = 1, Name = "Work" },
              new Category { Id = 2, Name = "Personal" },
              new Category { Id = 3, Name = "Shopping" },
              new Category { Id = 4, Name = "Goals" },
              new Category { Id = 5, Name = "Watchlist" },
              new Category { Id = 6, Name = "Random" },
              new Category { Id = 7, Name = "Uni" },
              new Category { Id = 8, Name = "School" },
              new Category { Id = 9, Name = "Chores" },
              new Category { Id = 10, Name = "Challenges" }
          );

            builder.Entity<Achievement>().HasData(
                new Achievement { Id = 1, Name = "First List", Description = "Create your first list" },
                new Achievement { Id = 2, Name = "First Todo", Description = "Add your first todo" },
                new Achievement { Id = 3, Name = "Complete a List", Description = "Complete all tasks in a list" },
                new Achievement { Id = 4, Name = "Complete Todo", Description = "Complete your first todo by checking all subtasks" },
                new Achievement { Id = 5, Name = "Complete 50 Todos Overall", Description = "Complete 50 todos in all lists combined" }
            );

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<ToDoList> ToDoLists { get; set; }
        public DbSet<ToDo> Todos { get; set; }
        public DbSet<Subtask> Subtasks { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<UserAchievement> UserAchievements { get; set; }
    }
}
