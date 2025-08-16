using Microsoft.EntityFrameworkCore;
using EtabsExtensions.Core.Models;
using EtabsExtensions.Infrastructure.Data.Configurations;

namespace EtabsExtensions.Infrastructure.Data;

public class TodoDbContext: DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options)
        : base(options)
    {
        
    }

    public DbSet<TodoItem> TodoItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfiguration(new TodoItemConfiguration());

        // Seed data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        var sampleTodos = new[]
        {
            new TodoItem
            {
                Id = 1,
                Title = "Learn .NET 9",
                Description = "Explore the new features in .NET 9",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            },
            new TodoItem
            {
                Id = 2,
                Title = "Build WPF App",
                Description = "Create a modern WPF application with MVVM",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            },
            new TodoItem
            {
                Id = 3,
                Title = "Setup CI/CD",
                Description = "Configure continuous integration and deployment",
                IsCompleted = true,
                CreatedAt = DateTime.UtcNow.AddDays(-7),
                CompletedAt = DateTime.UtcNow.AddDays(-2)
            }
        };

        // Ensure the database is created
        modelBuilder.Entity<TodoItem>().HasData(sampleTodos);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Fallback configuration - should not be used in production
            optionsBuilder.UseSqlite("Data Source=todo.db");
        }
    }
}
