using EtabsExtensions.Core.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtabsExtensions.Infrastructure.Data.Configurations;

public class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem>
{
    public void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.IsCompleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("datetime('now')");

        builder.Property(x => x.CompletedAt)
            .IsRequired(false);

        // Indexes
        builder.HasIndex(x => x.IsCompleted)
            .HasDatabaseName("IX_TodoItems_IsCompleted");

        builder.HasIndex(x => x.CreatedAt)
            .HasDatabaseName("IX_TodoItems_CreatedAt");
    }
}