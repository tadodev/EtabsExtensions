using EtabsExtensions.Core.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtabsExtensions.Infrastructure.Data.Configurations;

public class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem>
{
    public void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        throw new NotImplementedException();
    }
}